// **********************************************************************
//
// Copyright (c) 2003-2004 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

#include <Ice/Incoming.h>
#include <Ice/ObjectAdapter.h>
#include <Ice/ServantLocator.h>
#include <Ice/ServantManager.h>
#include <Ice/Object.h>
#include <Ice/Connection.h>
#include <Ice/LocalException.h>
#include <Ice/Instance.h>
#include <Ice/Properties.h>
#include <Ice/IdentityUtil.h>
#include <Ice/LoggerUtil.h>
#include <Ice/Protocol.h>
#include <IceUtil/StringUtil.h>

using namespace std;
using namespace Ice;
using namespace IceInternal;

IceInternal::IncomingBase::IncomingBase(Instance* instance, Connection* connection, 
					const ObjectAdapterPtr& adapter,
					const TransportInfoPtr& transport,
					bool response, Byte compress) :
    _response(response),
    _compress(compress),
    _os(instance),
    _connection(connection)
{
    _current.adapter = adapter;
    _current.transport = transport;
}

IceInternal::IncomingBase::IncomingBase(IncomingBase& in) :
    _current(in._current),
    _servant(in._servant),
    _locator(in._locator),
    _cookie(in._cookie),
    _response(in._response),
    _compress(in._compress),
    _os(in._os.instance()),
    _connection(in._connection)
{
    _os.swap(in._os);
}

void
IceInternal::IncomingBase::__finishInvoke()
{
    if(_locator && _servant)
    {
	_locator->finished(_current, _servant, _cookie);
    }
    
    //
    // Send a response if necessary. If we don't need to send a
    // response, we still need to tell the connection that we're
    // finished with dispatching.
    //
    if(_response)
    {
	_connection->sendResponse(&_os, _compress);
    }
    else
    {
	_connection->sendNoResponse();
    }
}

void
IceInternal::IncomingBase::__warning(const Exception& ex) const
{
    ostringstream str;
    str << ex;
    __warning(str.str());
}

void
IceInternal::IncomingBase::__warning(const string& msg) const
{
    Warning out(_os.instance()->logger());
    
    out << "dispatch exception: " << msg;
    out << "\nidentity: " << _current.id;
    out << "\nfacet: " << IceUtil::escapeString(_current.facet, "");
    out << "\noperation: " << _current.operation;
}

IceInternal::Incoming::Incoming(Instance* instance, Connection* connection, 
				const ObjectAdapterPtr& adapter,
				const TransportInfoPtr& transport,
				bool response, Byte compress) :
    IncomingBase(instance, connection, adapter, transport, response, compress),
    _is(instance)
{
}

void
IceInternal::Incoming::invoke(const ServantManagerPtr& servantManager)
{
    //
    // Read the current.
    //
    _current.id.__read(&_is);

    //
    // For compatibility with the old FacetPath.
    //
    vector<string> facetPath;
    _is.read(facetPath);
    string facet;
    if(!facetPath.empty())
    {
	if(facetPath.size() > 1)
	{
	    throw MarshalException(__FILE__, __LINE__);
	}
	facet.swap(facetPath[0]);
    }
    _current.facet.swap(facet);

    _is.read(_current.operation);

    Byte b;
    _is.read(b);
    _current.mode = static_cast<OperationMode>(b);

    Int sz;
    _is.readSize(sz);
    while(sz--)
    {
	pair<const string, string> pr;
	_is.read(const_cast<string&>(pr.first));
	_is.read(pr.second);
	_current.ctx.insert(_current.ctx.end(), pr);
    }

    _is.startReadEncaps();

    if(_response)
    {
	assert(_os.b.size() == headerSize + 4); // Dispatch status position.
	_os.write(static_cast<Byte>(0));
	_os.startWriteEncaps();
    }

    DispatchStatus status;

    //
    // Don't put the code above into the try block below. Exceptions
    // in the code above are considered fatal, and must propagate to
    // the caller of this operation.
    //

    try
    {
	if(servantManager)
	{
	    _servant = servantManager->findServant(_current.id, _current.facet);
	    
	    if(!_servant && !_current.id.category.empty())
	    {
		_locator = servantManager->findServantLocator(_current.id.category);
		if(_locator)
		{
		    _servant = _locator->locate(_current, _cookie);
		}
	    }
	    
	    if(!_servant)
	    {
		_locator = servantManager->findServantLocator("");
		if(_locator)
		{
		    _servant = _locator->locate(_current, _cookie);
		}
	    }
	}
	    
	if(!_servant)
	{
	    if(servantManager && servantManager->hasServant(_current.id))
	    {
		status = DispatchFacetNotExist;
	    }
	    else
	    {
		status = DispatchObjectNotExist;
	    }
	}
	else
	{
	    status = _servant->__dispatch(*this, _current);
	}
    }
    catch(RequestFailedException& ex)
    {
	_is.endReadEncaps();

	if(ex.id.name.empty())
	{
	    ex.id = _current.id;
	}
	
	if(ex.facet.empty() && !_current.facet.empty())
	{
	    ex.facet = _current.facet;
	}
	
	if(ex.operation.empty() && !_current.operation.empty())
	{
	    ex.operation = _current.operation;
	}

	if(_os.instance()->properties()->getPropertyAsIntWithDefault("Ice.Warn.Dispatch", 1) > 1)
	{
	    __warning(ex);
	}

	if(_response)
	{
	    _os.endWriteEncaps();
	    _os.b.resize(headerSize + 4); // Dispatch status position.
	    if(dynamic_cast<ObjectNotExistException*>(&ex))
	    {
		_os.write(static_cast<Byte>(DispatchObjectNotExist));
	    }
	    else if(dynamic_cast<FacetNotExistException*>(&ex))
	    {
		_os.write(static_cast<Byte>(DispatchFacetNotExist));
	    }
	    else if(dynamic_cast<OperationNotExistException*>(&ex))
	    {
		_os.write(static_cast<Byte>(DispatchOperationNotExist));
	    }
	    else
	    {
		assert(false);
	    }

	    ex.id.__write(&_os);

	    //
	    // For compatibility with the old FacetPath.
	    //
	    if(ex.facet.empty())
	    {
		_os.write(vector<string>());
	    }
	    else
	    {
		vector<string> facetPath;
		facetPath.push_back(ex.facet);
		_os.write(facetPath);
	    }

	    _os.write(ex.operation);
	}

	//
	// Must be called last, so that if an exception is raised,
	// this function is definitely *not* called.
	//
	__finishInvoke();
	return;
    }
    catch(const UnknownLocalException& ex)
    {
	_is.endReadEncaps();

	if(_os.instance()->properties()->getPropertyAsIntWithDefault("Ice.Warn.Dispatch", 1) > 0)
	{
	    __warning(ex);
	}

	if(_response)
	{
	    _os.endWriteEncaps();
	    _os.b.resize(headerSize + 4); // Dispatch status position.
	    _os.write(static_cast<Byte>(DispatchUnknownLocalException));
	    _os.write(ex.unknown);
	}

	//
	// Must be called last, so that if an exception is raised,
	// this function is definitely *not* called.
	//
	__finishInvoke();
	return;
    }
    catch(const UnknownUserException& ex)
    {
	_is.endReadEncaps();

	if(_os.instance()->properties()->getPropertyAsIntWithDefault("Ice.Warn.Dispatch", 1) > 0)
	{
	    __warning(ex);
	}

	if(_response)
	{
	    _os.endWriteEncaps();
	    _os.b.resize(headerSize + 4); // Dispatch status position.
	    _os.write(static_cast<Byte>(DispatchUnknownUserException));
	    _os.write(ex.unknown);
	}

	//
	// Must be called last, so that if an exception is raised,
	// this function is definitely *not* called.
	//
	__finishInvoke();
	return;
    }
    catch(const UnknownException& ex)
    {
	_is.endReadEncaps();

	if(_os.instance()->properties()->getPropertyAsIntWithDefault("Ice.Warn.Dispatch", 1) > 0)
	{
	    __warning(ex);
	}

	if(_response)
	{
	    _os.endWriteEncaps();
	    _os.b.resize(headerSize + 4); // Dispatch status position.
	    _os.write(static_cast<Byte>(DispatchUnknownException));
	    _os.write(ex.unknown);
	}

	//
	// Must be called last, so that if an exception is raised,
	// this function is definitely *not* called.
	//
	__finishInvoke();
	return;
    }
    catch(const LocalException& ex)
    {
	_is.endReadEncaps();

	if(_os.instance()->properties()->getPropertyAsIntWithDefault("Ice.Warn.Dispatch", 1) > 0)
	{
	    __warning(ex);
	}

	if(_response)
	{
	    _os.endWriteEncaps();
	    _os.b.resize(headerSize + 4); // Dispatch status position.
	    _os.write(static_cast<Byte>(DispatchUnknownLocalException));
	    ostringstream str;
	    str << ex;
	    _os.write(str.str());
	}

	//
	// Must be called last, so that if an exception is raised,
	// this function is definitely *not* called.
	//
	__finishInvoke();
	return;
    }
    catch(const UserException& ex)
    {
	_is.endReadEncaps();

	if(_os.instance()->properties()->getPropertyAsIntWithDefault("Ice.Warn.Dispatch", 1) > 0)
	{
	    __warning(ex);
	}

	if(_response)
	{
	    _os.endWriteEncaps();
	    _os.b.resize(headerSize + 4); // Dispatch status position.
	    _os.write(static_cast<Byte>(DispatchUnknownUserException));
	    ostringstream str;
	    str << ex;
	    _os.write(str.str());
	}

	//
	// Must be called last, so that if an exception is raised,
	// this function is definitely *not* called.
	//
	__finishInvoke();
	return;
    }
    catch(const Exception& ex)
    {
	_is.endReadEncaps();

	if(_os.instance()->properties()->getPropertyAsIntWithDefault("Ice.Warn.Dispatch", 1) > 0)
	{
	    __warning(ex);
	}

	if(_response)
	{
	    _os.endWriteEncaps();
	    _os.b.resize(headerSize + 4); // Dispatch status position.
	    _os.write(static_cast<Byte>(DispatchUnknownException));
	    ostringstream str;
	    str << ex;
	    _os.write(str.str());
	}

	//
	// Must be called last, so that if an exception is raised,
	// this function is definitely *not* called.
	//
	__finishInvoke();
	return;
    }
    catch(const std::exception& ex)
    {
	_is.endReadEncaps();

	if(_os.instance()->properties()->getPropertyAsIntWithDefault("Ice.Warn.Dispatch", 1) > 0)
	{
	    __warning(string("std::exception: ") + ex.what());
	}

	if(_response)
	{
	    _os.endWriteEncaps();
	    _os.b.resize(headerSize + 4); // Dispatch status position.
	    _os.write(static_cast<Byte>(DispatchUnknownException));
	    ostringstream str;
	    str << "std::exception: " << ex.what();
	    _os.write(str.str());
	}

	//
	// Must be called last, so that if an exception is raised,
	// this function is definitely *not* called.
	//
	__finishInvoke();
	return;
    }
    catch(...)
    {
	_is.endReadEncaps();

	if(_os.instance()->properties()->getPropertyAsIntWithDefault("Ice.Warn.Dispatch", 1) > 0)
	{
	    __warning("unknown c++ exception");
	}

	if(_response)
	{
	    _os.endWriteEncaps();
	    _os.b.resize(headerSize + 4); // Dispatch status position.
	    _os.write(static_cast<Byte>(DispatchUnknownException));
	    string reason = "unknown c++ exception";
	    _os.write(reason);
	}

	//
	// Must be called last, so that if an exception is raised,
	// this function is definitely *not* called.
	//
	__finishInvoke();
	return;
    }

    //
    // Don't put the code below into the try block above. Exceptions
    // in the code below are considered fatal, and must propagate to
    // the caller of this operation.
    //
    
    _is.endReadEncaps();

    //
    // DispatchAsync is "pseudo dispatch status", used internally only
    // to indicate async dispatch.
    //
    if(status == DispatchAsync)
    {
	//
	// If this was an asynchronous dispatch, we're done here.  We
	// do *not* call __finishInvoke(), because the call is not
	// finished yet.
	//
	return;
    }

    if(_response)
    {
	_os.endWriteEncaps();
	
	if(status != DispatchOK && status != DispatchUserException)
	{
	    assert(status == DispatchObjectNotExist ||
		   status == DispatchFacetNotExist ||
		   status == DispatchOperationNotExist);
	    
	    _os.b.resize(headerSize + 4); // Dispatch status position.
	    _os.write(static_cast<Byte>(status));
	    
	    _current.id.__write(&_os);

	    //
	    // For compatibility with the old FacetPath.
	    //
	    if(_current.facet.empty())
	    {
		_os.write(vector<string>());
	    }
	    else
	    {
		vector<string> facetPath;
		facetPath.push_back(_current.facet);
		_os.write(facetPath);
	    }

	    _os.write(_current.operation);
	}
	else
	{
	    *(_os.b.begin() + headerSize + 4) = static_cast<Byte>(status); // Dispatch status position.
	}
    }

    //
    // Must be called last, so that if an exception is raised,
    // this function is definitely *not* called.
    //
    __finishInvoke();
}
