using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DatabaseVersionControl.UnitTests.ConfigLoaders
{
    public class TestHelper
    {
        public static void BuildAssert(string name, object asset)
        {
            var builder = new Builder(2, 3, o => Console.Out.WriteLine(o));
            builder.BuildAssert(name, asset, 0);
        }

        public static void BuildAssertMethods(string findalbumSeachcherbelieve,
                                              string lastfmintegrationFindalbumBelieveCher, string func,
                                              Func<object> functionCall)
        {
            object result = functionCall();
            string type = result.GetType().Name;
            string resultVar = string.Format(@"_{0}{1}", findalbumSeachcherbelieve,
                                             lastfmintegrationFindalbumBelieveCher);
            string resultMethod = string.Format(@"{0}{1}Setup", findalbumSeachcherbelieve,
                                                lastfmintegrationFindalbumBelieveCher);
            Action<string> output = (o => Console.Out.WriteLine(o));
            output(string.Format(@"private {0} {1} = null;", type, resultVar));
            output(
                string.Format(
                    "private {0} {3}() {{ \r\n if ({1} == null) {{ \r\n {1} = {2}; \r\n }} return {1}; \r\n  }}\r\n",
                    type, resultVar, func, resultMethod));
            string methodCall = "" + resultMethod + "();\r\n";
            string testMethodName = string.Format(@"{0}_{1}_", findalbumSeachcherbelieve,
                                                  lastfmintegrationFindalbumBelieveCher);
            var builder = new Builder(2, 3,
                                      (n, o) =>
                                      output(
                                          string.Format(
                                              "[Test,Category(\"Integration\")]\r\npublic void {2}() {{ var value = {1} {0};\r\n }} ",
                                              o, methodCall, testMethodName + Regex.Replace(n, @"[\[\]\.]", ""))));
            builder.BuildAssert("value", result, 0);
        }

        #region Nested type: Builder

        public class Builder
        {
            private readonly Action<string, string> _action;

            public Builder(int maxlevel, int maxArray, Action<string> action)
            {
                _action = ((n, v) => action(v));
                Maxlevel = maxlevel;
                MaxArray = maxArray;
            }

            public Builder(int maxlevel, int maxArray, Action<string, string> action)
            {
                _action = action;
                Maxlevel = maxlevel;
                MaxArray = maxArray;
            }

            public int MaxArray { get; private set; }
            public int Maxlevel { get; private set; }

            public void BuildAssert(string name, object asset, int level)
            {
                if (level > Maxlevel)
                    return;
                if (asset != null)
                {
                    PropertyInfo[] properties = asset.GetType().GetProperties();
                    foreach (PropertyInfo info in properties)
                    {
                        if (!info.GetGetMethod().IsStatic)
                            WriteOut(name, info.Name, info.GetValue(asset, null), level);
                    }
                    FieldInfo[] fields = asset.GetType().GetFields();
                    foreach (FieldInfo info in fields)
                    {
                        if (!info.IsStatic)
                            WriteOut(name, info.Name, info.GetValue(asset), level);
                    }
                }
                else
                    _action(name, string.Format("Assert.That({0},Is.Null,\"invalid value for {0}\");", name));
            }

            public void BuildAssert(string name, Array asset)
            {
                BuildAssert(name, asset, 0);
            }   

            public void BuildAssert(string name, Array asset, int level)
            {
                _action(name, string.Format("Assert.That({0}.Length,Is.EqualTo({1}),\"invalid value for {0}\");", name,
                                            asset.Length));
                int i = 0;
                foreach (object fa in asset)
                {
                    if (i > MaxArray) break;
                    string fullname = name + "[" + i + "]";
                    BuildAssert(fullname, fa, level + 1);
                    i++;
                }
            }

            public void BuildAssert(string name, string asset)
            {
                _action(name,
                        string.Format("Assert.That({0},Is.EqualTo({1}),\"invalid value for {0}\");", name,
                                      "\"" + asset + "\""));
            }

            public void BuildAssert(string name, IDictionary asset)
            {
                BuildAssert(name, asset, 0);
            }

            public void BuildAssert(string name, IDictionary asset, int level)
            {
                _action(name,
                        string.Format("Assert.That({0}.Count,Is.EqualTo({1}),\"invalid value for {0}\");", name,
                                      asset.Count));
                int i = 0;
                foreach (object fa in asset)
                {
                    if (i > MaxArray) break;
                    string fullname = name + "[" + i + "]";
                    _action(name, string.Format("var {0} = {1};", fullname, fullname));
                    BuildAssert(fullname, fa, level + 1);
                    i++;
                }
            }


            public void BuildAssert(string name, ICollection asset)
            {
                BuildAssert(name, asset, 0);
            }

            public void BuildAssert(string name, ICollection asset, int level)
            {
                _action(name,
                        string.Format("Assert.That({0}.Count,Is.EqualTo({1}),\"invalid value for {0}\");", name,
                                      asset.Count));
                int i = 0;
                foreach (object fa in asset)
                {
                    if (i > MaxArray) break;
                    string fullname = name + "[" + i + "]";
                    BuildAssert(fullname, fa, level + 1);
                    i++;
                }
            }

            public void BuildAssert(string name, Enum asset)
            {
                _action(name,
                        string.Format("Assert.That({0},Is.EqualTo({1}),\"invalid value for enum {0}\");", name,
                                      asset.GetType().FullName + "." + asset));
            }

            public void BuildAssert(string name, TimeSpan asset)
            {
                _action(name,
                        string.Format(
                            "Assert.That({0}.ToString(),Is.EqualTo(\"{1}\"),\"invalid value for timespan {0}\");", name,
                            asset));
            }

            public void BuildAssert(string name, int asset)
            {
                _action(name,
                        string.Format("Assert.That({0},Is.EqualTo({1}),\"invalid value for int {0}\");", name, asset));
            }

            public void BuildAssert(string name, long asset)
            {
                _action(name,
                        string.Format("Assert.That({0},Is.EqualTo({1}),\"invalid value for int {0}\");", name, asset));
            }

            private void WriteOut(string name, string valueName, object info, int level)
            {
                string newName = name + "." + valueName;
                if (info is Enum)
                    BuildAssert(newName, info as Enum);
                else if (info is string)
                    BuildAssert(newName, info as string);
                else if (info is int)
                    BuildAssert(newName, Convert.ToInt32(info));
                else if (info is long)
                    BuildAssert(newName, Convert.ToInt64(info));
                else if (info is TimeSpan)
                    BuildAssert(newName, (TimeSpan)info);
                else if (info is Array)
                    BuildAssert(newName, info as Array, level);
                else if (info is IDictionary)
                    BuildAssert(newName, info as IDictionary, level);
                else if (info is ICollection)
                    BuildAssert(newName, info as ICollection, level);
                else
                {
                    //Console.Out.WriteLine("TestHelper.BuildAssert(\"{0}\",{0});", newName);
                    //BuildAssert(newName+".ToString()", info.ToString());
                    BuildAssert(newName, info, level + 1);
                }
            }
        }

        #endregion
    }
}