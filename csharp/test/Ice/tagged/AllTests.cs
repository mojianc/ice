//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

using System.Linq;
using System.Collections.Generic;
using System.IO;
using Test;

namespace ZeroC.Ice.Test.Tagged
{
    public class AllTests
    {
        public static IInitialPrx Run(TestHelper helper)
        {
            Communicator? communicator = helper.Communicator;
            TestHelper.Assert(communicator != null);

            TextWriter output = helper.Output;
            var initial = IInitialPrx.Parse(helper.GetTestProxy("initial", 0), communicator);

            output.Write("testing tagged data members... ");
            output.Flush();

            var oo1 = new OneTagged();
            TestHelper.Assert(!oo1.A.HasValue);
            oo1.A = 15;
            TestHelper.Assert(oo1.A.HasValue && oo1.A == 15);

            var oo2 = new OneTagged(16);
            TestHelper.Assert(oo2.A.HasValue && oo2.A == 16);

            var mo1 = new MultiTagged();
            mo1.A = 15;
            mo1.B = true;
            mo1.C = 19;
            mo1.D = 78;
            mo1.E = 99;
            mo1.F = (float)5.5;
            mo1.G = 1.0;
            mo1.H = "test";
            mo1.I = MyEnum.MyEnumMember;
            mo1.Bs = new byte[] { 5 };
            mo1.Ss = new string[] { "test", "test2" };
            mo1.Iid = new Dictionary<int, int>
            {
                { 4, 3 }
            };
            mo1.Sid = new Dictionary<string, int>
            {
                { "test", 10 }
            };
            var fs = new FixedStruct();
            fs.M = 78;
            mo1.Fs = fs;
            var vs1 = new VarStruct();
            vs1.M = "hello";
            mo1.Vs = vs1;

            mo1.Shs = new short[] { 1 };
            mo1.Es = new MyEnum[] { MyEnum.MyEnumMember, MyEnum.MyEnumMember };
            mo1.Fss = new FixedStruct[] { fs };
            mo1.Vss = new VarStruct[] { vs1 };

            mo1.Ied = new Dictionary<int, MyEnum>
            {
                { 4, MyEnum.MyEnumMember }
            };
            mo1.Ifsd = new Dictionary<int, FixedStruct>
            {
                { 4, fs }
            };
            mo1.Ivsd = new Dictionary<int, VarStruct>
            {
                { 5, vs1 }
            };

            mo1.Bos = new bool[] { false, true, false };

            mo1.Us = 321;
            mo1.Ui = 123;
            mo1.Ul = 10_000_000_000_000;
            mo1.Vi = 3_000_000;
            mo1.Vl = -10_000_000_000_000;
            mo1.Vui = 123;
            mo1.Vul = 20_000_000_000_000;

            mo1.Uss = new ushort[] { 6, 1, 327 };
            mo1.Vuls = new ulong[] { 100, 500, 1 };
            mo1.Vil = new List<int> { 2, -300, 0 };

            TestHelper.Assert(mo1.A == 15);
            TestHelper.Assert(mo1.B == true);
            TestHelper.Assert(mo1.C == 19);
            TestHelper.Assert(mo1.D == 78);
            TestHelper.Assert(mo1.E == 99);
            TestHelper.Assert(mo1.F == (float)5.5);
            TestHelper.Assert(mo1.G == 1.0);
            TestHelper.Assert(mo1.H.Equals("test"));
            TestHelper.Assert(mo1.I == MyEnum.MyEnumMember);
            TestHelper.Assert(Enumerable.SequenceEqual(mo1.Bs, new byte[] { 5 }));
            TestHelper.Assert(Enumerable.SequenceEqual(mo1.Ss, new string[] { "test", "test2" }));
            TestHelper.Assert(mo1.Iid[4] == 3);
            TestHelper.Assert(mo1.Sid["test"] == 10);
            TestHelper.Assert(mo1.Fs.Equals(new FixedStruct(78)));
            TestHelper.Assert(mo1.Vs.Equals(new VarStruct("hello")));

            TestHelper.Assert(mo1.Shs[0] == 1);
            TestHelper.Assert(mo1.Es[0] == MyEnum.MyEnumMember && mo1.Es[1] == MyEnum.MyEnumMember);
            TestHelper.Assert(mo1.Fss[0].Equals(new FixedStruct(78)));
            TestHelper.Assert(mo1.Vss[0].Equals(new VarStruct("hello")));

            TestHelper.Assert(mo1.Ied[4] == MyEnum.MyEnumMember);
            TestHelper.Assert(mo1.Ifsd[4].Equals(new FixedStruct(78)));
            TestHelper.Assert(mo1.Ivsd[5].Equals(new VarStruct("hello")));

            TestHelper.Assert(Enumerable.SequenceEqual(mo1.Bos, new bool[] { false, true, false }));

            output.WriteLine("ok");

            output.Write("testing marshaling... ");
            output.Flush();

            var oo4 = (OneTagged?)initial.PingPong(new OneTagged());
            TestHelper.Assert(oo4 != null && !oo4.A.HasValue);

            var oo5 = (OneTagged?)initial.PingPong(oo1);
            TestHelper.Assert(oo5 != null && oo1.A == oo5.A);

            var mo4 = (MultiTagged?)initial.PingPong(new MultiTagged());
            TestHelper.Assert(mo4 != null);
            TestHelper.Assert(mo4.A == null);
            TestHelper.Assert(mo4.B == null);
            TestHelper.Assert(mo4.C == null);
            TestHelper.Assert(mo4.D == null);
            TestHelper.Assert(mo4.E == null);
            TestHelper.Assert(mo4.F == null);
            TestHelper.Assert(mo4.G == null);
            TestHelper.Assert(mo4.H == null);
            TestHelper.Assert(mo4.I == null);
            TestHelper.Assert(mo4.Bs == null);
            TestHelper.Assert(mo4.Ss == null);
            TestHelper.Assert(mo4.Iid == null);
            TestHelper.Assert(mo4.Sid == null);
            TestHelper.Assert(mo4.Fs == null);
            TestHelper.Assert(mo4.Vs == null);

            TestHelper.Assert(mo4.Shs == null);
            TestHelper.Assert(mo4.Es == null);
            TestHelper.Assert(mo4.Fss == null);
            TestHelper.Assert(mo4.Vss == null);

            TestHelper.Assert(mo4.Ied == null);
            TestHelper.Assert(mo4.Ifsd == null);
            TestHelper.Assert(mo4.Ivsd == null);

            TestHelper.Assert(mo4.Bos == null);

            TestHelper.Assert(mo4.Us == null);
            TestHelper.Assert(mo4.Ui == null);
            TestHelper.Assert(mo4.Ul == null);
            TestHelper.Assert(mo4.Vi == null);
            TestHelper.Assert(mo4.Vl == null);
            TestHelper.Assert(mo4.Vui == null);
            TestHelper.Assert(mo4.Vul == null);

            TestHelper.Assert(mo4.Uss == null);
            TestHelper.Assert(mo4.Vuls == null);
            TestHelper.Assert(mo4.Vil == null);

            var mo5 = (MultiTagged?)initial.PingPong(mo1);
            TestHelper.Assert(mo5 != null);
            TestHelper.Assert(mo5.A == mo1.A);
            TestHelper.Assert(mo5.B == mo1.B);
            TestHelper.Assert(mo5.C == mo1.C);
            TestHelper.Assert(mo5.D == mo1.D);
            TestHelper.Assert(mo5.E == mo1.E);
            TestHelper.Assert(mo5.F == mo1.F);
            TestHelper.Assert(mo5.G == mo1.G);
            TestHelper.Assert(mo5.H!.Equals(mo1.H));
            TestHelper.Assert(mo5.I == mo1.I);
            TestHelper.Assert(Enumerable.SequenceEqual(mo5.Bs!, mo1.Bs));
            TestHelper.Assert(Enumerable.SequenceEqual(mo5.Ss!, mo1.Ss));
            TestHelper.Assert(mo5.Iid != null && mo5.Iid[4] == 3);
            TestHelper.Assert(mo5.Sid != null && mo5.Sid["test"] == 10);
            TestHelper.Assert(mo5.Fs.Equals(mo1.Fs));
            TestHelper.Assert(mo5.Vs.Equals(mo1.Vs));
            TestHelper.Assert(Enumerable.SequenceEqual(mo5.Shs!, mo1.Shs));
            TestHelper.Assert(mo5.Es != null && mo5.Es[0] == MyEnum.MyEnumMember && mo1.Es[1] == MyEnum.MyEnumMember);
            TestHelper.Assert(mo5.Fss != null && mo5.Fss[0].Equals(new FixedStruct(78)));
            TestHelper.Assert(mo5.Vss != null && mo5.Vss[0].Equals(new VarStruct("hello")));

            TestHelper.Assert(mo5.Ied != null && mo5.Ied[4] == MyEnum.MyEnumMember);
            TestHelper.Assert(mo5.Ifsd != null && mo5.Ifsd[4].Equals(new FixedStruct(78)));
            TestHelper.Assert(mo5.Ivsd != null && mo5.Ivsd[5].Equals(new VarStruct("hello")));

            TestHelper.Assert(mo5.Us == mo1.Us);
            TestHelper.Assert(mo5.Ui == mo1.Ui);
            TestHelper.Assert(mo5.Ul == mo1.Ul);
            TestHelper.Assert(mo5.Vi == mo1.Vi);
            TestHelper.Assert(mo5.Vl == mo1.Vl);
            TestHelper.Assert(mo5.Vui == mo1.Vui);
            TestHelper.Assert(mo5.Vul == mo1.Vul);

            TestHelper.Assert(mo5.Uss!.SequenceEqual(mo1.Uss));
            TestHelper.Assert(mo5.Vuls!.SequenceEqual(mo1.Vuls));
            TestHelper.Assert(mo5.Vil!.SequenceEqual(mo1.Vil));

            // Clear the first half of the tagged members
            var mo6 = new MultiTagged();
            mo6.B = mo5.B;
            mo6.D = mo5.D;
            mo6.F = mo5.F;
            mo6.H = mo5.H;
            mo6.Bs = mo5.Bs;
            mo6.Iid = mo5.Iid;
            mo6.Fs = mo5.Fs;
            mo6.Shs = mo5.Shs;
            mo6.Fss = mo5.Fss;
            mo6.Ifsd = mo5.Ifsd;
            mo6.Bos = mo5.Bos;

            var mo7 = (MultiTagged?)initial.PingPong(mo6);
            TestHelper.Assert(mo7 != null);
            TestHelper.Assert(mo7.A == null);
            TestHelper.Assert(mo7.B.Equals(mo1.B));
            TestHelper.Assert(mo7.C == null);
            TestHelper.Assert(mo7.D.Equals(mo1.D));
            TestHelper.Assert(mo7.E == null);
            TestHelper.Assert(mo7.F.Equals(mo1.F));
            TestHelper.Assert(mo7.G == null);
            TestHelper.Assert(mo7.H != null && mo7.H.Equals(mo1.H));
            TestHelper.Assert(mo7.I == null);
            TestHelper.Assert(Enumerable.SequenceEqual(mo7.Bs!, mo1.Bs));
            TestHelper.Assert(mo7.Ss == null);
            TestHelper.Assert(mo7.Iid != null && mo7.Iid[4] == 3);
            TestHelper.Assert(mo7.Sid == null);
            TestHelper.Assert(mo7.Fs.Equals(mo1.Fs));
            TestHelper.Assert(mo7.Vs == null);

            TestHelper.Assert(Enumerable.SequenceEqual(mo7.Shs!, mo1.Shs));
            TestHelper.Assert(mo7.Es == null);
            TestHelper.Assert(mo7.Fss != null && mo7.Fss[0].Equals(new FixedStruct(78)));
            TestHelper.Assert(mo7.Vss == null);

            TestHelper.Assert(mo7.Ied == null);
            TestHelper.Assert(mo7.Ifsd != null && mo7.Ifsd[4].Equals(new FixedStruct(78)));
            TestHelper.Assert(mo7.Ivsd == null);

            TestHelper.Assert(Enumerable.SequenceEqual(mo7.Bos!, new bool[] { false, true, false }));

            // Clear the second half of the tagged members
            var mo8 = new MultiTagged();
            mo8.A = mo5.A;
            mo8.C = mo5.C;
            mo8.E = mo5.E;
            mo8.G = mo5.G;
            mo8.I = mo5.I;
            mo8.Ss = mo5.Ss;
            mo8.Sid = mo5.Sid;
            mo8.Vs = mo5.Vs;

            mo8.Es = mo5.Es;
            mo8.Vss = mo5.Vss;

            mo8.Ied = mo5.Ied;
            mo8.Ivsd = mo5.Ivsd;

            var mo9 = (MultiTagged?)initial.PingPong(mo8);
            TestHelper.Assert(mo9 != null);
            TestHelper.Assert(mo9.A.Equals(mo1.A));
            TestHelper.Assert(!mo9.B.HasValue);
            TestHelper.Assert(mo9.C.Equals(mo1.C));
            TestHelper.Assert(!mo9.D.HasValue);
            TestHelper.Assert(mo9.E.Equals(mo1.E));
            TestHelper.Assert(!mo9.F.HasValue);
            TestHelper.Assert(mo9.G.Equals(mo1.G));
            TestHelper.Assert(mo9.H == null);
            TestHelper.Assert(mo9.I.Equals(mo1.I));
            TestHelper.Assert(mo9.Bs == null);
            TestHelper.Assert(Enumerable.SequenceEqual(mo9.Ss!, mo1.Ss));
            TestHelper.Assert(mo9.Iid == null);
            TestHelper.Assert(mo9.Sid != null && mo9.Sid["test"] == 10);
            TestHelper.Assert(mo9.Fs == null);
            TestHelper.Assert(mo9.Vs.Equals(mo1.Vs));

            TestHelper.Assert(mo9.Shs == null);
            TestHelper.Assert(mo9.Es != null && mo9.Es[0] == MyEnum.MyEnumMember && mo9.Es[1] == MyEnum.MyEnumMember);
            TestHelper.Assert(mo9.Fss == null);
            TestHelper.Assert(mo9.Vss != null && mo9.Vss[0].Equals(new VarStruct("hello")));

            TestHelper.Assert(mo9.Ied != null && mo9.Ied[4] == MyEnum.MyEnumMember);
            TestHelper.Assert(mo9.Ifsd == null);
            TestHelper.Assert(mo9.Ivsd != null && mo9.Ivsd[5].Equals(new VarStruct("hello")));

            TestHelper.Assert(mo9.Bos == null);

            {
                var owc1 = new TaggedWithCustom();
                owc1.L = new List<SmallStruct>
                {
                    new SmallStruct(5),
                    new SmallStruct(6),
                    new SmallStruct(7)
                };
                owc1.S = new ClassVarStruct(5);
                var owc2 = (TaggedWithCustom?)initial.PingPong(owc1);
                TestHelper.Assert(owc2 != null);
                TestHelper.Assert(owc2.L != null);
                TestHelper.Assert(Enumerable.SequenceEqual(owc1.L, owc2.L));
                TestHelper.Assert(owc2.S != null && owc2.S.Value.A == 5);
            }

            /* TODO: rewrite test without factories

            //
            // Send a request using blobjects. Upon receival, we don't read
            // any of the tagged members. This ensures the tagged members
            // are skipped even if the receiver knows nothing about them.
            //
            factory.setEnabled(true);
            OutputStream os = new OutputStream(communicator);
            os.StartEncapsulation();
            os.WriteNullableClass(oo1);
            os.EndEncapsulation();
            byte[] inEncaps = os.Finished();
            byte[] outEncaps;
            test(initial.Invoke("pingPong", idempotent: false, inEncaps, out outEncaps));
            InputStream
            responseFrame.InputStream.StartEncapsulation();
            ReadNullableClassCallbackI cb = new ReadNullableClassCallbackI();
            responseFrame.InputStream.ReadNullableClass(cb.invoke);
            responseFrame.InputStream.EndEncapsulation();
            test(cb.obj != null && cb.obj is TestClassReader);

            os = new OutputStream(communicator);
            os.StartEncapsulation();
            os.WriteNullableClass(mo1);
            os.EndEncapsulation();
            inEncaps = os.Finished();
            test(initial.Invoke("pingPong", idempotent: false, inEncaps, out outEncaps));

            responseFrame.InputStream.StartEncapsulation();
            responseFrame.InputStream.ReadNullableClass(cb.invoke);
            responseFrame.InputStream.EndEncapsulation();
            test(cb.obj != null && cb.obj is TestClassReader);
            factory.setEnabled(false);
            */

            initial.OpVoid();

            var requestFrame = OutgoingRequestFrame.WithParamList(
                initial, "opVoid", idempotent: false, compress: false, format: default, context: null, (15, "test"),
                (OutputStream ostr, in (int n, string s) value) =>
                {
                    ostr.WriteTaggedInt(1, value.n);
                    ostr.WriteTaggedString(1, value.s); // duplicate tag ignored by the server
                });

            TestHelper.Assert(initial.Invoke(requestFrame).ResultType == ResultType.Success);

            output.WriteLine("ok");

            output.Write("testing marshaling of large containers with fixed size elements... ");
            output.Flush();
            var mc = new MultiTagged();

            mc.Bs = new byte[1000];
            mc.Shs = new short[300];

            mc.Fss = new FixedStruct[300];
            for (int i = 0; i < 300; ++i)
            {
                mc.Fss[i] = new FixedStruct();
            }

            mc.Ifsd = new Dictionary<int, FixedStruct>();
            for (int i = 0; i < 300; ++i)
            {
                mc.Ifsd.Add(i, new FixedStruct());
            }

            mc = (MultiTagged?)initial.PingPong(mc);
            TestHelper.Assert(mc != null);
            TestHelper.Assert(mc.Bs!.Length == 1000);
            TestHelper.Assert(mc.Shs!.Length == 300);
            TestHelper.Assert(mc.Fss!.Length == 300);
            TestHelper.Assert(mc.Ifsd!.Count == 300);

            /*
            factory.setEnabled(true);
            os = new OutputStream(communicator);
            os.StartEncapsulation();
            os.WriteNullableClass(mc);
            os.EndEncapsulation();
            inEncaps = os.Finished();
            test(initial.Invoke("pingPong", idempotent: false, inEncaps, out outEncaps));

            responseFrame.InputStream.StartEncapsulation();
            responseFrame.InputStream.ReadNullableClass(cb.invoke);
            responseFrame.InputStream.EndEncapsulation();
            test(cb.obj != null && cb.obj is TestClassReader);
            factory.setEnabled(false);
            */

            output.WriteLine("ok");

            output.Write("testing tag marshaling... ");
            output.Flush();
            {
                var b = new B();
                var b2 = (B?)initial.PingPong(b);
                TestHelper.Assert(b2 != null);
                TestHelper.Assert(!b2.Ma.HasValue);
                TestHelper.Assert(!b2.Mb.HasValue);
                TestHelper.Assert(!b2.Mc.HasValue);

                b.Ma = 10;
                b.Mb = 11;
                b.Mc = 12;
                b.Md = 13;

                b2 = (B?)initial.PingPong(b);
                TestHelper.Assert(b2 != null);
                TestHelper.Assert(b2.Ma == 10);
                TestHelper.Assert(b2.Mb == 11);
                TestHelper.Assert(b2.Mc == 12);
                TestHelper.Assert(b2.Md == 13);

                /*
                factory.setEnabled(true);
                os = new OutputStream(communicator);
                os.StartEncapsulation();
                os.WriteNullableClass(b);
                os.EndEncapsulation();
                inEncaps = os.Finished();
                test(initial.Invoke("pingPong", idempotent: false, inEncaps, out outEncaps));

                responseFrame.InputStream.StartEncapsulation();
                responseFrame.InputStream.ReadNullableClass(cb.invoke);
                responseFrame.InputStream.EndEncapsulation();
                test(cb.obj != null);
                factory.setEnabled(false);
                */

            }
            output.WriteLine("ok");

            output.Write("testing tagged with default values... ");
            output.Flush();
            {
                var wd = (WD?)initial.PingPong(new WD());
                TestHelper.Assert(wd != null);
                TestHelper.Assert(wd.A == 5);
                TestHelper.Assert(wd.S! == "test");
                wd.A = null;
                wd.S = null;
                wd = (WD?)initial.PingPong(wd);
                TestHelper.Assert(wd != null);
                // When a tagged member is set to null (equivalent to not set) explicitly, it remains null / not set,
                // even when it has a default value. This is consistent with the behavior for non-tagged optional
                // data members with default values.
                TestHelper.Assert(wd.A == null);
                TestHelper.Assert(wd.S == null);
            }
            output.WriteLine("ok");

            output.Write("testing tagged parameters... ");
            output.Flush();
            {
                byte? p1 = null;
                (byte? p2, byte? p3) = initial.OpByte(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpByte(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = 56;
                (p2, p3) = initial.OpByte(p1);
                TestHelper.Assert(p2 == 56 && p3 == 56);
                (byte? ReturnValue, byte? p3) r = initial.OpByteAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 56 && r.p3 == 56);
                (p2, p3) = initial.OpByte(p1);
                TestHelper.Assert(p2 == 56 && p3 == 56);
                r = initial.OpByteAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 56 && r.p3 == 56);

                (p2, p3) = initial.OpByte(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial, "opByte",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null, p1,
                    (OutputStream ostr, byte? p1) => ostr.WriteTaggedByte(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    {
                        byte? b1 = istr.ReadTaggedByte(1);
                        byte? b2 = istr.ReadTaggedByte(2);
                        return (b1, b2);
                    });
                TestHelper.Assert(p2 == 56);
                TestHelper.Assert(p3 == 56);
            }

            {
                bool? p1 = null;
                (bool? p2, bool? p3) = initial.OpBool(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpBool(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = true;
                (p2, p3) = initial.OpBool(p1);
                TestHelper.Assert(p2 == true && p3 == true);
                (bool? ReturnValue, bool? p3) r = initial.OpBoolAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == true && r.p3 == true);
                (p2, p3) = initial.OpBool(true);
                TestHelper.Assert(p2 == true && p3 == true);
                r = initial.OpBoolAsync(true).Result;
                TestHelper.Assert(r.ReturnValue == true && r.p3 == true);

                (p2, p3) = initial.OpBool(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opBool",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null, p1,
                    (OutputStream ostr, bool? p1) => ostr.WriteTaggedBool(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                {
                    bool? b1 = istr.ReadTaggedBool(1);
                    bool? b2 = istr.ReadTaggedBool(2);
                    return (b1, b2);
                });
                TestHelper.Assert(p2 == true);
                TestHelper.Assert(p3 == true);
            }

            {
                short? p1 = null;
                (short? p2, short? p3) = initial.OpShort(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpShort(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = 56;
                (p2, p3) = initial.OpShort(p1);
                TestHelper.Assert(p2 == 56 && p3 == 56);
                (short? ReturnValue, short? p3) r = initial.OpShortAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 56 && r.p3 == 56);
                (p2, p3) = initial.OpShort(p1);
                TestHelper.Assert(p2 == 56 && p3 == 56);
                r = initial.OpShortAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 56 && r.p3 == 56);

                (p2, p3) = initial.OpShort(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opShort",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null, p1,
                    (OutputStream ostr, short? p1) => ostr.WriteTaggedShort(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    {
                        short? s1 = istr.ReadTaggedShort(1);
                        short? s2 = istr.ReadTaggedShort(2);
                        return (s1, s2);
                    });
                TestHelper.Assert(p2 == 56);
                TestHelper.Assert(p3 == 56);
            }

            {
                int? p1 = null;
                (int? p2, int? p3) = initial.OpInt(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpInt(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = 56;
                (p2, p3) = initial.OpInt(p1);
                TestHelper.Assert(p2 == 56 && p3 == 56);
                (int? ReturnValue, int? p3) r = initial.OpIntAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 56 && r.p3 == 56);
                (p2, p3) = initial.OpInt(p1);
                TestHelper.Assert(p2 == 56 && p3 == 56);
                r = initial.OpIntAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 56 && r.p3 == 56);

                (p2, p3) = initial.OpInt(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opInt",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, int? p1) => ostr.WriteTaggedInt(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                {
                    int? i1 = istr.ReadTaggedInt(1);
                    int? i2 = istr.ReadTaggedInt(2);
                    return (i1, i2);
                });
                TestHelper.Assert(p2 == 56);
                TestHelper.Assert(p3 == 56);
            }

            {
                long? p1 = null;
                (long? p2, long? p3) = initial.OpLong(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpLong(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = 56;
                (p2, p3) = initial.OpLong(p1);
                TestHelper.Assert(p2 == 56 && p3 == 56);
                (long? ReturnValue, long? p3) r = initial.OpLongAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 56 && r.p3 == 56);
                (p2, p3) = initial.OpLong(p1);
                TestHelper.Assert(p2 == 56 && p3 == 56);
                r = initial.OpLongAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 56 && r.p3 == 56);

                (p2, p3) = initial.OpLong(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial, "opLong",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, long? p1) => ostr.WriteTaggedLong(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                {
                    long? l1 = istr.ReadTaggedLong(1);
                    long? l2 = istr.ReadTaggedLong(2);
                    return (l1, l2);
                });
                TestHelper.Assert(p2 == 56);
                TestHelper.Assert(p3 == 56);
            }

            {
                float? p1 = null;
                (float? p2, float? p3) = initial.OpFloat(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpFloat(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = 1.0f;
                (p2, p3) = initial.OpFloat(p1);
                TestHelper.Assert(p2 == 1.0f && p3 == 1.0f);
                (float? ReturnValue, float? p3) r = initial.OpFloatAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 1.0f && r.p3 == 1.0f);
                (p2, p3) = initial.OpFloat(p1);
                TestHelper.Assert(p2 == 1.0f && p3 == 1.0f);
                r = initial.OpFloatAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 1.0f && r.p3 == 1.0f);

                (p2, p3) = initial.OpFloat(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial, "opFloat",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, float? p1) => ostr.WriteTaggedFloat(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    {
                        float? f1 = istr.ReadTaggedFloat(1);
                        float? f2 = istr.ReadTaggedFloat(2);
                        return (f1, f2);
                    });
                TestHelper.Assert(p2 == 1.0f);
                TestHelper.Assert(p3 == 1.0f);
            }

            {
                double? p1 = null;
                (double? p2, double? p3) = initial.OpDouble(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpDouble(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = 1.0;
                (p2, p3) = initial.OpDouble(p1);
                TestHelper.Assert(p2 == 1.0 && p3 == 1.0);
                (double? ReturnValue, double? p3) r = initial.OpDoubleAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 1.0 && r.p3 == 1.0);
                (p2, p3) = initial.OpDouble(p1);
                TestHelper.Assert(p2 == 1.0 && p3 == 1.0);
                r = initial.OpDoubleAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == 1.0 && r.p3 == 1.0);

                (p2, p3) = initial.OpDouble(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opDouble",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, double? p1) => ostr.WriteTaggedDouble(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    {
                        double? d1 = istr.ReadTaggedDouble(1);
                        double? d2 = istr.ReadTaggedDouble(2);
                        return (d1, d2);
                    });
                TestHelper.Assert(p2 == 1.0);
                TestHelper.Assert(p3 == 1.0);
            }

            {
                string? p1 = null;
                (string? p2, string? p3) = initial.OpString(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpString(null);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpString(null); // Implicitly converts to string>(null)
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = "test";
                (p2, p3) = initial.OpString(p1);
                TestHelper.Assert(p2 == "test" && p3 == "test");
                (string? ReturnValue, string? p3) r = initial.OpStringAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == "test" && r.p3 == "test");
                (p2, p3) = initial.OpString(p1);
                TestHelper.Assert(p2 == "test" && p3 == "test");
                r = initial.OpStringAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == "test" && r.p3 == "test");

                (p2, p3) = initial.OpString(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opString",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, string? p1) => ostr.WriteTaggedString(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                {
                    string? s1 = istr.ReadTaggedString(1);
                    string? s2 = istr.ReadTaggedString(2);
                    return (s1, s2);
                });
                TestHelper.Assert(p2 == "test");
                TestHelper.Assert(p3 == "test");
            }

            {
                MyEnum? p1 = null;
                (MyEnum? p2, MyEnum? p3) = initial.OpMyEnum(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpMyEnum(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = MyEnum.MyEnumMember;
                (p2, p3) = initial.OpMyEnum(p1);
                TestHelper.Assert(p2 == MyEnum.MyEnumMember && p3 == MyEnum.MyEnumMember);
                (MyEnum? ReturnValue, MyEnum? p3) r = initial.OpMyEnumAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == MyEnum.MyEnumMember && r.p3 == MyEnum.MyEnumMember);
                (p2, p3) = initial.OpMyEnum(p1);
                TestHelper.Assert(p2 == MyEnum.MyEnumMember && p3 == MyEnum.MyEnumMember);
                r = initial.OpMyEnumAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue == MyEnum.MyEnumMember && r.p3 == MyEnum.MyEnumMember);

                (p2, p3) = initial.OpMyEnum(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opMyEnum",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, MyEnum? p1) => ostr.WriteTaggedSize(1, (int?) p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedSize(1)?.AsMyEnum(), istr.ReadTaggedSize(2)?.AsMyEnum()));
                TestHelper.Assert(p2 == MyEnum.MyEnumMember);
                TestHelper.Assert(p3 == MyEnum.MyEnumMember);
            }

            {
                SmallStruct? p1 = null;
                (SmallStruct? p2, SmallStruct? p3) = initial.OpSmallStruct(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpSmallStruct(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = new SmallStruct(56);
                (p2, p3) = initial.OpSmallStruct(p1);
                TestHelper.Assert(p2!.Value.M == 56 && p3!.Value.M == 56);
                (SmallStruct? ReturnValue, SmallStruct? p3) r = initial.OpSmallStructAsync(p1).Result;
                TestHelper.Assert(p2!.Value.M == 56 && p3!.Value.M == 56);
                (p2, p3) = initial.OpSmallStruct(p1);
                TestHelper.Assert(p2!.Value.M == 56 && p3!.Value.M == 56);
                r = initial.OpSmallStructAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue!.Value.M == 56 && r.p3!.Value.M == 56);

                (p2, p3) = initial.OpSmallStruct(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opSmallStruct",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, SmallStruct? p1) => ostr.WriteTaggedStruct(1, p1, 1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedStruct(1, fixedSize: true, istr => new SmallStruct(istr)),
                        istr.ReadTaggedStruct(2, fixedSize: true, istr => new SmallStruct(istr))));

                TestHelper.Assert(p2!.Value.M == 56);
                TestHelper.Assert(p3!.Value.M == 56);
            }

            {
                FixedStruct? p1 = null;
                (FixedStruct? p2, FixedStruct? p3) = initial.OpFixedStruct(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpFixedStruct(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = new FixedStruct(56);
                (p2, p3) = initial.OpFixedStruct(p1);
                TestHelper.Assert(p2!.Value.M == 56 && p3!.Value.M == 56);
                (FixedStruct? ReturnValue, FixedStruct? p3) r = initial.OpFixedStructAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue!.Value.M == 56 && r.p3!.Value.M == 56);
                (p2, p3) = initial.OpFixedStruct(p1);
                TestHelper.Assert(p2!.Value.M == 56 && p3!.Value.M == 56);
                r = initial.OpFixedStructAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue!.Value.M == 56 && r.p3!.Value.M == 56);

                (p2, p3) = initial.OpFixedStruct(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opFixedStruct",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, FixedStruct? p1) => ostr.WriteTaggedStruct(1, p1, 4));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedStruct(1, fixedSize: true, istr => new FixedStruct(istr)),
                        istr.ReadTaggedStruct(2, fixedSize: true, istr => new FixedStruct(istr))));
                TestHelper.Assert(p2!.Value.M == 56);
                TestHelper.Assert(p3!.Value.M == 56);
            }

            {
                VarStruct? p1 = null;
                (VarStruct? p2, VarStruct? p3) = initial.OpVarStruct(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpVarStruct(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = new VarStruct("test");
                (p2, p3) = initial.OpVarStruct(p1);
                TestHelper.Assert(p2!.Value.M.Equals("test") && p3!.Value.M.Equals("test"));

                // Test null struct
                (p2, p3) = initial.OpVarStruct(null);
                TestHelper.Assert(p2 == null && p3 == null);

                (VarStruct? ReturnValue, VarStruct? p3) r = initial.OpVarStructAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue!.Value.M.Equals("test") && r.p3!.Value.M.Equals("test"));
                (p2, p3) = initial.OpVarStruct(p1);
                TestHelper.Assert(p2!.Value.M.Equals("test") && p3!.Value.M.Equals("test"));
                r = initial.OpVarStructAsync(p1).Result;
                TestHelper.Assert(r.ReturnValue!.Value.M.Equals("test") && r.p3!.Value.M.Equals("test"));

                (p2, p3) = initial.OpVarStruct(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opVarStruct",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, VarStruct? p1) =>
                    {
                        TestHelper.Assert(p1 != null);
                        ostr.WriteTaggedStruct(1, p1);
                    });

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedStruct(1, fixedSize: false, istr => new VarStruct(istr)),
                        istr.ReadTaggedStruct(2, fixedSize: false, istr => new VarStruct(istr))));
                TestHelper.Assert(p2!.Value.M.Equals("test"));
                TestHelper.Assert(p3!.Value.M.Equals("test"));

                // TODO: why are we testing this here?
                /* F f = new F();
                f.af = new A();
                f.af.requiredA = 56;
                f.ae = f.af;

                ostr = new OutputStream(communicator);
                ostr.StartEncapsulation();
                ostr.WriteTagged(1, TaggedFormat.Class);
                ostr.WriteNullableClass(f);
                ostr.WriteTagged(2, TaggedFormat.Class);
                ostr.WriteNullableClass(f.ae);
                ostr.EndEncapsulation();
                var inEncaps = ostr.ToArray();

                var istr = new InputStream(communicator, inEncaps);
                istr.StartEncapsulation();
                test(istr.ReadTagged(2, TaggedFormat.Class));
                var a = istr.ReadNullableClass<A>();
                istr.EndEncapsulation();
                test(a != null && a.requiredA == 56);*/
            }

            {
                byte[]? p1 = null;
                (byte[]? p2, byte[]? p3) = initial.OpByteSeq(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpByteSeq(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(x => (byte)56).ToArray();
                (p2, p3) = initial.OpByteSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpByteSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpByteSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpByteSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpByteSeq(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opByteSeq",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, byte[]? p1) => ostr.WriteTaggedArray(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedArray<byte>(1), istr.ReadTaggedArray<byte>(2)));

                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                bool[]? p1 = null;
                (bool[]? p2, bool[]? p3) = initial.OpBoolSeq(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpBoolSeq(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => true).ToArray();
                (p2, p3) = initial.OpBoolSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpBoolSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpBoolSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpBoolSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpBoolSeq(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opBoolSeq",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, bool[]? p1) => ostr.WriteTaggedArray(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedArray<bool>(1), istr.ReadTaggedArray<bool>(2)));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                short[]? p1 = null;
                (short[]? p2, short[]? p3) = initial.OpShortSeq(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpShortSeq(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => (short)56).ToArray();
                (p2, p3) = initial.OpShortSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpShortSeqAsync(p1).Result;

                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpShortSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpShortSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpShortSeq(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opShortSeq",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, short[]? p1) => ostr.WriteTaggedArray(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedArray<short>(1), istr.ReadTaggedArray<short>(2)));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                int[]? p1 = null;
                (int[]? p2, int[]? p3) = initial.OpIntSeq(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpIntSeq(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => 56).ToArray();
                (p2, p3) = initial.OpIntSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpIntSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpIntSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpIntSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpIntSeq(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opIntSeq",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null, p1,
                    (OutputStream ostr, int[]? p1) => ostr.WriteTaggedArray(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedArray<int>(1), istr.ReadTaggedArray<int>(2)));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                long[]? p1 = null;
                (long[]? p2, long[]? p3) = initial.OpLongSeq(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpLongSeq(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => 56L).ToArray();
                (p2, p3) = initial.OpLongSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpLongSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpLongSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpLongSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpLongSeq(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opLongSeq",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, long[]? p1) => ostr.WriteTaggedArray(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedArray<long>(1), istr.ReadTaggedArray<long>(2)));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                float[]? p1 = null;
                (float[]? p2, float[]? p3) = initial.OpFloatSeq(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpFloatSeq(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => 1.0f).ToArray();
                (p2, p3) = initial.OpFloatSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpFloatSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpFloatSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpFloatSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpFloatSeq(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opFloatSeq",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, float[]? p1) => ostr.WriteTaggedArray(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedArray<float>(1), istr.ReadTaggedArray<float>(2)));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                double[]? p1 = null;
                (double[]? p2, double[]? p3) = initial.OpDoubleSeq(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpDoubleSeq(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 100).Select(_ => 1.0).ToArray();
                (p2, p3) = initial.OpDoubleSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpDoubleSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpDoubleSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpDoubleSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpDoubleSeq(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opDoubleSeq",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, double[]? p1) => ostr.WriteTaggedArray(1, p1));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedArray<double>(1), istr.ReadTaggedArray<double>(2)));

                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                string[]? p1 = null;
                (string[]? p2, string[]? p3) = initial.OpStringSeq(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpStringSeq(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 10).Select(_ => "test1").ToArray();
                (p2, p3) = initial.OpStringSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpStringSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpStringSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpStringSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpStringSeq(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opStringSeq",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, string[]? p1) =>
                        ostr.WriteTaggedSequence(1, p1, (ost, s) => ostr.WriteString(s)));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedArray(1, 1, fixedSize: false, istr => istr.ReadString()),
                       istr.ReadTaggedArray(2, 1, fixedSize: false, istr => istr.ReadString())));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                SmallStruct[]? p1 = null;
                (SmallStruct[]? p2, SmallStruct[]? p3) = initial.OpSmallStructSeq(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpSmallStructSeq(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 10).Select(_ => new SmallStruct()).ToArray();
                (p2, p3) = initial.OpSmallStructSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpSmallStructSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpSmallStructSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpSmallStructSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpSmallStructSeq(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opSmallStructSeq",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, SmallStruct[]? p1) => ostr.WriteTaggedSequence(1, p1, 1,
                        (ostr, st) => ostr.WriteStruct(st)));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedArray(1, 1, fixedSize: true, istr => new SmallStruct(istr)),
                        istr.ReadTaggedArray(2, 1, fixedSize: true, istr => new SmallStruct(istr))));

                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                List<SmallStruct>? p1 = null;
                (List<SmallStruct>? p2, List<SmallStruct>? p3) = initial.OpSmallStructList(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpSmallStructList(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = new List<SmallStruct>();
                for (int i = 0; i < 10; ++i)
                {
                    p1.Add(new SmallStruct());
                }
                (p2, p3) = initial.OpSmallStructList(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1));
                (p2, p3) = initial.OpSmallStructListAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1));
                (p2, p3) = initial.OpSmallStructList(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1));
                (p2, p3) = initial.OpSmallStructListAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1));

                (p2, p3) = initial.OpSmallStructList(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opSmallStructList",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, List<SmallStruct>? p1) => ostr.WriteTaggedSequence(1, p1, 1,
                        (ostr, st) => ostr.WriteStruct(st)));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    {
                        List<SmallStruct>? list1 =
                            istr.ReadTaggedSequence(1, 1, fixedSize: true, istr => new SmallStruct(istr)) is
                                ICollection<SmallStruct> collection1 ? new List<SmallStruct>(collection1) : null;

                        List<SmallStruct>? list2 =
                            istr.ReadTaggedSequence(2, 1, fixedSize: true, istr => new SmallStruct(istr)) is
                                ICollection<SmallStruct> collection2 ? new List<SmallStruct>(collection2) : null;

                        return (list1, list2);
                    });
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));

            }

            {
                FixedStruct[]? p1 = null;
                (FixedStruct[]? p2, FixedStruct[]? p3) = initial.OpFixedStructSeq(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpFixedStructSeq(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 10).Select(_ => new FixedStruct()).ToArray();
                (p2, p3) = initial.OpFixedStructSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpFixedStructSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpFixedStructSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpFixedStructSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpFixedStructSeq(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opFixedStructSeq",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, FixedStruct[]? p1) => ostr.WriteTaggedSequence(1, p1, 4,
                        (ostr, st) => ostr.WriteStruct(st)));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedArray(1, 4, fixedSize: true, istr => new FixedStruct(istr)),
                        istr.ReadTaggedArray(2, 4, fixedSize: true, istr => new FixedStruct(istr))));

                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                LinkedList<FixedStruct>? p1 = null;
                (LinkedList<FixedStruct>? p2, LinkedList<FixedStruct>? p3) = initial.OpFixedStructList(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpFixedStructList(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = new LinkedList<FixedStruct>();
                for (int i = 0; i < 10; ++i)
                {
                    p1.AddLast(new FixedStruct());
                }
                (p2, p3) = initial.OpFixedStructList(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpFixedStructListAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpFixedStructList(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpFixedStructListAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpFixedStructList(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opFixedStructList",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, LinkedList<FixedStruct>? p1) => ostr.WriteTaggedSequence(1, p1, 4,
                        (ostr, st) => ostr.WriteStruct(st)));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    {
                        LinkedList<FixedStruct>? list1 =
                            istr.ReadTaggedSequence(1, 4, fixedSize: true, istr => new FixedStruct(istr)) is
                                ICollection<FixedStruct> collection1 ? new LinkedList<FixedStruct>(collection1) : null;

                        LinkedList<FixedStruct>? list2 =
                            istr.ReadTaggedSequence(2, 4, fixedSize: true, istr => new FixedStruct(istr)) is
                                ICollection<FixedStruct> collection2 ? new LinkedList<FixedStruct>(collection2) : null;

                        return (list1, list2);
                    });
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                VarStruct[]? p1 = null;
                (VarStruct[]? p2, VarStruct[]? p3) = initial.OpVarStructSeq(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpVarStructSeq(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = Enumerable.Range(0, 10).Select(_ => new VarStruct("")).ToArray();
                (p2, p3) = initial.OpVarStructSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpVarStructSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpVarStructSeq(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpVarStructSeqAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpVarStructSeq(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opVarStructSeq",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, VarStruct[]? p1) =>
                        ostr.WriteTaggedSequence(1, p1, (ostr, vs) => ostr.WriteStruct(vs)));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                      (istr.ReadTaggedArray(1, 1, fixedSize: false, istr => new VarStruct(istr)),
                        istr.ReadTaggedArray(2, 1, fixedSize: false, istr => new VarStruct(istr))));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                Dictionary<int, int>? p1 = null;
                (Dictionary<int, int>? p2, Dictionary<int, int>? p3) = initial.OpIntIntDict(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpIntIntDict(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = new Dictionary<int, int>
                {
                    { 1, 2 },
                    { 2, 3 }
                };
                (p2, p3) = initial.OpIntIntDict(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpIntIntDictAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpIntIntDict(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpIntIntDictAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpIntIntDict(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opIntIntDict",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null, p1,
                    (OutputStream ostr, Dictionary<int, int>? p1) => ostr.WriteTaggedDictionary(1, p1, 8,
                        (ostr, k) => ostr.WriteInt(k), (ostr, v) => ostr.WriteInt(v)));

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                    (istr.ReadTaggedDictionary(1, 4, 4, fixedSize: true,
                        istr => istr.ReadInt(), istr => istr.ReadInt()),
                     istr.ReadTaggedDictionary(2, 4, 4, fixedSize: true,
                         istr => istr.ReadInt(), istr => istr.ReadInt())));

                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }

            {
                Dictionary<string, int>? p1 = null;
                (Dictionary<string, int>? p2, Dictionary<string, int>? p3) = initial.OpStringIntDict(p1);
                TestHelper.Assert(p2 == null && p3 == null);
                (p2, p3) = initial.OpStringIntDict(null);
                TestHelper.Assert(p2 == null && p3 == null);

                p1 = new Dictionary<string, int>
                {
                    { "1", 1 },
                    { "2", 2 }
                };
                (p2, p3) = initial.OpStringIntDict(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpStringIntDictAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpStringIntDict(p1);
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                (p2, p3) = initial.OpStringIntDictAsync(p1).Result;
                TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));

                (p2, p3) = initial.OpStringIntDict(null);
                TestHelper.Assert(p2 == null && p3 == null); // Ensure out parameter is cleared.

                requestFrame = OutgoingRequestFrame.WithSingleParam(
                    initial,
                    "opStringIntDict",
                    idempotent: false,
                    compress: false,
                    format: default,
                    context: null,
                    p1,
                    (OutputStream ostr, Dictionary<string, int>? p1) =>
                    {
                        TestHelper.Assert(p1 != null);
                        ostr.WriteTaggedDictionary(1, p1,
                            (ostr, k) => ostr.WriteString(k), (ostr, v) => ostr.WriteInt(v));
                    });

                IncomingResponseFrame responseFrame = initial.Invoke(requestFrame);
                (p2, p3) = responseFrame.ReadReturnValue(communicator, istr =>
                      (istr.ReadTaggedDictionary(1, 1, 4, fixedSize: false, istr => istr.ReadString(),
                                                                            istr => istr.ReadInt()),
                       istr.ReadTaggedDictionary(2, 1, 4, fixedSize: false, istr => istr.ReadString(),
                                                                            istr => istr.ReadInt())));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p2!));
                TestHelper.Assert(Enumerable.SequenceEqual(p1, p3!));
            }
            output.WriteLine("ok");

            output.Write("testing exceptions with tagged members... ");
            output.Flush();
            {
                try
                {
                    int? a = null;
                    string? b = null;
                    VarStruct? vs = null;
                    initial.OpTaggedException(a, b, vs);
                }
                catch (TaggedException ex)
                {
                    TestHelper.Assert(ex.A == null); // don't use default value for 'a' data member since set explicitly
                                                     // to null by server
                    TestHelper.Assert(ex.B == null);
                    TestHelper.Assert(ex.Vs == null);
                }

                try
                {
                    int? a = 30;
                    string? b = "test";
                    VarStruct? vs = new VarStruct("hello");
                    initial.OpTaggedException(a, b, vs);
                }
                catch (TaggedException ex)
                {
                    TestHelper.Assert(ex.A == 30);
                    TestHelper.Assert(ex.B == "test");
                    TestHelper.Assert(ex.Vs.Equals(new VarStruct("hello")));
                }

                try
                {
                    int? a = null;
                    string? b = null;
                    VarStruct? vs = null;
                    initial.OpDerivedException(a, b, vs);
                }
                catch (DerivedException ex)
                {
                    TestHelper.Assert(ex.A == null); // don't use default value for 'a' data member since set explicitly
                                                     // to null by server
                    TestHelper.Assert(ex.B == null);
                    TestHelper.Assert(ex.Vs == null);
                    TestHelper.Assert(ex.Ss == null);
                    TestHelper.Assert(ex.Vs2 == null);
                }

                try
                {
                    int? a = 30;
                    string? b = "test2";
                    VarStruct? vs = new VarStruct("hello2");
                    initial.OpDerivedException(a, b, vs);
                }
                catch (DerivedException ex)
                {
                    TestHelper.Assert(ex.A == 30);
                    TestHelper.Assert(ex.B == "test2");
                    TestHelper.Assert(ex.Vs.Equals(new VarStruct("hello2")));
                    TestHelper.Assert(ex.Ss == "test2");
                    TestHelper.Assert(ex.Vs2.Equals(new VarStruct("hello2")));
                }

                try
                {
                    int? a = null;
                    string? b = null;
                    VarStruct? vs = null;
                    initial.OpRequiredException(a, b, vs);
                }
                catch (RequiredException ex)
                {
                    TestHelper.Assert(ex.A == null);
                    TestHelper.Assert(ex.B == null);
                    TestHelper.Assert(ex.Vs == null);
                    TestHelper.Assert(ex.Ss == "test");
                    TestHelper.Assert(ex.Vs2.Equals(new VarStruct("")));
                }

                try
                {
                    int? a = 30;
                    string? b = "test2";
                    VarStruct? vs = new VarStruct("hello2");
                    initial.OpRequiredException(a, b, vs);
                }
                catch (RequiredException ex)
                {
                    TestHelper.Assert(ex.A == 30);
                    TestHelper.Assert(ex.B == "test2");
                    TestHelper.Assert(ex.Vs.Equals(new VarStruct("hello2")));
                    TestHelper.Assert(ex.Ss == "test2");
                    TestHelper.Assert(ex.Vs2.Equals(new VarStruct("hello2")));
                }
            }
            output.WriteLine("ok");

            output.Write("testing tagged members with marshaled results... ");
            output.Flush();
            {
                TestHelper.Assert(initial.OpMStruct1() != null);
                TestHelper.Assert(initial.OpMDict1() != null);
                TestHelper.Assert(initial.OpMSeq1() != null);

                {
                    SmallStruct? p1, p2, p3;
                    (p3, p2) = initial.OpMStruct2(null);
                    TestHelper.Assert(p2 == null && p3 == null);

                    p1 = new SmallStruct();
                    (p3, p2) = initial.OpMStruct2(p1);
                    TestHelper.Assert(p2.Equals(p1) && p3.Equals(p1));
                }
                {
                    string[]? p1, p2, p3;
                    (p3, p2) = initial.OpMSeq2(null);
                    TestHelper.Assert(p2 == null && p3 == null);

                    p1 = new string[1] { "hello" };
                    (p3, p2) = initial.OpMSeq2(p1);
                    TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                }
                {
                    Dictionary<string, int>? p1, p2, p3;
                    (p3, p2) = initial.OpMDict2(null);
                    TestHelper.Assert(p2 == null && p3 == null);

                    p1 = new Dictionary<string, int>
                    {
                        ["test"] = 54
                    };
                    (p3, p2) = initial.OpMDict2(p1);
                    TestHelper.Assert(Enumerable.SequenceEqual(p2!, p1) && Enumerable.SequenceEqual(p3!, p1));
                }
            }
            output.WriteLine("ok");

            return initial;
        }
    }
}