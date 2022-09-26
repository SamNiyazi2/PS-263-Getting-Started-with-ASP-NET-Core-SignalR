 
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

// Copied to:
// 01/12/2021 04:23 pm - SSN - Jumped in to copy and use for modifying launchsetting.json in to C:\Sams_P\PS\aspdotnet-core-signalr-getting-started\d\04\demos\ConsoleClient
// 01/12/2021 04:25 pm - SSN - Copied from https://devsitesindex20190127.azurewebsites.net/CodeReferences/Details?id=2203121
//                                         C:\Sams_P\PS\aspdotnet-core-tag-helpers\d\dd\M4-Built-In-Tag-Helpers\Clips-1-to-10\Models\Util_Gen.cs

    // Renamed namespace and class from Util_Gen
// namespace WebAppTagHelper.Models
namespace ConsoleClient
{
//    public static class Util_Gen
    public static class JSON_Util
    {

        public static string getEnvironmentVariable(string lookupKey)
        {

            string fileName = Environment.CurrentDirectory + @"\..\..\..\..\wiredbrain\properties\launchsettings.json";

            if (!File.Exists(fileName))
            {
                return null;
            }


            string fileContent = System.IO.File.ReadAllText(fileName);


            string returnValue = null;

            JToken json_parent_2 = JObject.Parse(fileContent);
            var fieldsCollector = new JsonFieldsCollector(json_parent_2);
            var fields = fieldsCollector.GetAllFields();

            foreach (var field in fields)
            {
                if (field.Key == lookupKey)
                {
                    returnValue = field.Value.ToString();
                }
            }

            return returnValue;
        }






        public static string setEnvironmentVariable()
        {

            string fileName = Environment.CurrentDirectory + @"\..\..\..\..\wiredbrain\properties\launchsettings.json";

            if (!File.Exists(fileName))
            {
                return null;
            }

            string fileNameOutput_format = Environment.CurrentDirectory + @"\..\..\..\..\wiredbrain\properties\launchsettings_{0:000}.json";

            string fileNameOutput = createBackupfileName(fileNameOutput_format);

            string fileContent = System.IO.File.ReadAllText(fileName);

          
            JToken json_parent_2 = JObject.Parse(fileContent);
 

            int changeCount = 0;

           
             

            addProp(json_parent_2, new[] { "one" }, "valu2-123");


            JToken temp1 = json_parent_2["one"];
            ////////////////////////////////////////////     JToken temp2 = jtokenUtil.json_parent_2["one"]["BBB"];

            // temp1.Root.Last.AddAfterSelf(new JProperty("two", "valu12"));
            addProp(json_parent_2, new[] { "two" }, "value-345");
         //   addProp(json_parent_2, new[] { "one", "two", "three" }, "value-999");


            //            jtokenUtil.json_parent_2.Last.AddAfterSelf(new JProperty("one"]["three"], "valu13"));


            changeCount += 1;


            //updateJsonResults.Add(r1);
            //updateJsonResults.Add(r2);

            //            if (updateJsonResults.Any(r => r.didChange))
            if (changeCount > 0)
            {
                File.Move(fileName, fileNameOutput);

                string result = Newtonsoft.Json.JsonConvert.SerializeObject(json_parent_2);
                System.IO.File.WriteAllText(fileName, result);
            }

            StringBuilder sb = new StringBuilder();


            var fieldsCollector = new JsonFieldsCollector(json_parent_2);
            var fields = fieldsCollector.GetAllFields();

            foreach (var field in fields)
                sb.AppendLine($"{field.Key}: '{field.Value}'");

            return  string.Format("<pre>{0}</pre>", sb.ToString());
        }



        private static void addProp(JToken jtoken, string[] keys, string value)
        {

            JToken jtoken_current = jtoken;

            for (int ndx = 0; ndx < keys.Length; ndx++)
            {
                Debug.WriteLine($"20201218-1109: addProp [{keys[0]}]");

                string parentKey = $"['{string.Join("']['", keys.Take(ndx + 1))}']";
                string childKey = $"['{string.Join("']['", keys.Take(ndx + 2))}']";


                Debug.WriteLine($"ssn-20201218-0944-A: Adding [{parentKey}] [{value}]");
                Debug.WriteLine($"ssn-20201218-0944-B: Adding [{childKey}] [{value}]");


                 JToken jtoken_found = jtoken_current.SelectToken(parentKey);
                ///////////////////////////JObject jtoken_found = (JObject)jtoken_current.SelectToken(parentKey);


                if (jtoken_found == null)
                {
                    Debug.WriteLine($"ssn-20201218-0944: Adding [{ndx}-{keys.Length}] [{parentKey}] [{value}]");
                    //jtoken.Last.AddAfterSelf(new JProperty(key, "valu12"));
                    // jtoken.Last.AddAfterSelf(new JObject(key, "valu12"));
                    // jtoken.Last.AddAfterSelf(new JObject(concatinatedKeys, "valu12"));
                    if (ndx + 1 < keys.Length)
                    {
                        if (ndx + 1 == keys.Length)
                        {
                            jtoken_current.Last.AddAfterSelf(new JProperty(parentKey, new JProperty(childKey, "xxxxx")));

                        }
                        else
                        {
                            // jtoken_current.Last.AddAfterSelf(new JObject(parentKey, new JObject(childKey )));
                            jtoken_current.Last.AddAfterSelf(new JObject(parentKey));
                        }

                        //JObject child = (JObject)jtoken_current.SelectToken(key);
                        //child.Add(key, new JProperty(key, "xxx"));
                        //jtoken_current = jtoken_current.SelectToken(key).Children().FirstOrDefault();
                        jtoken_current = jtoken_current.SelectToken(childKey);

                    }
                    else
                    {
                        jtoken_current.Last.AddAfterSelf(new JProperty(parentKey, "ggggggggg"));

                    }


                }
                else
                {
                    Debug.WriteLine("Aleady added. Updating ");
                    JValue oldJValue = (JValue)jtoken_found;

                    updateJsonKeyValue(jtoken_found, oldJValue.Value, value);
                }

            }

        }

        private static string createBackupfileName(string fileNameOutput_format)
        {
            string fileNameOutput = null;

            int fileVersion = 1;
            while (fileVersion < 100)
            {
                fileNameOutput = string.Format(fileNameOutput_format, fileVersion);
                if (!File.Exists(fileNameOutput)) break;
                fileNameOutput = null;
                fileVersion++;

            }

            return fileNameOutput;
        }

       

        private static bool updateJsonKeyValue(JToken json_child, object oldValue_1, string newValue)
        {

            bool didChange = false;
            JValue local_JToken = null;

            bool? existingValueAndPassedInOldValueDoMatch = null;


            if (json_child != null)
            {

                if (oldValue_1 != null)
                {
                    existingValueAndPassedInOldValueDoMatch = ((JValue)json_child).Value.Equals(oldValue_1);
                }


                //                    if (((JValue)json_child).ToString() == oldValue)
                if (!existingValueAndPassedInOldValueDoMatch.HasValue || existingValueAndPassedInOldValueDoMatch.Value)
                {
                    local_JToken = (JValue)json_child;

                    local_JToken.Value = newValue;

                    didChange = true;
                }
                else
                {

                }
            }

            // return new UpdateJsonResult { didChange = didChange, jtoken = local_JToken };
            return didChange;

        }

        //}


        private static string processJsonObject(dynamic myClass)
        {
            foreach (dynamic o in myClass.Children())
            {
                processJsonObject(o);
            }

            return null;
        }


        // https://riptutorial.com/csharp/example/32164/collect-all-fields-of-json-object
        private class JsonFieldsCollector
        {
            private readonly Dictionary<string, JValue> fields;

            public JsonFieldsCollector(JToken token)
            {
                fields = new Dictionary<string, JValue>();
                CollectFields(token);
            }

            private void CollectFields(JToken jToken)
            {
                switch (jToken.Type)
                {
                    case JTokenType.Object:
                        foreach (var child in jToken.Children<JProperty>())
                            CollectFields(child);
                        break;
                    case JTokenType.Array:
                        foreach (var child in jToken.Children())
                            CollectFields(child);
                        break;
                    case JTokenType.Property:
                        CollectFields(((JProperty)jToken).Value);
                        break;
                    default:
                        fields.Add(jToken.Path, (JValue)jToken);
                        break;
                }
            }

            public IEnumerable<KeyValuePair<string, JValue>> GetAllFields() => fields;
        }


    }



}
