using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISCCTests
{
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Xunit.Sdk;

    public class JsonFileDataAttribute : DataAttribute
    {
        private const string JsonInputsProperty = "inputs";
        private const string JsonOutputsProperty = "outputs";
        private readonly string _filePath;

        /// <summary>
        /// Load data from a JSON file as the data source for a theory
        /// </summary>
        /// <param name="filePath">The absolute or relative path to the JSON file to load</param>
        public JsonFileDataAttribute(string filePath)
            : this(filePath, null) { }

        /// <summary>
        /// Load data from a JSON file as the data source for a theory
        /// </summary>
        /// <param name="filePath">The absolute or relative path to the JSON file to load</param>
        public JsonFileDataAttribute(string filePath, string propertyName)
        {
            _filePath = filePath;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null) { throw new ArgumentNullException(nameof(testMethod)); }

            // Get the absolute path to the JSON file
            var path = Path.IsPathRooted(_filePath)
                ? _filePath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), _filePath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            // Load the file
            var fileData = File.ReadAllText(_filePath);
            return HandleTestParameters(testMethod, JsonConvert.DeserializeObject<JObject>(fileData));
            
        }

        /// <summary>
        /// Breakout Test JSON for the specific test into inputs and outputs.
        /// </summary>
        /// <param name="testMethod">Method of the Unit Test</param>
        /// <param name="deserialized">Parsed JSON object representing the <testfile>.json</param>
        /// <returns></returns>
        private IEnumerable<object[]> HandleTestParameters(MethodInfo testMethod, JObject deserialized)
        {
            var jobjectMatcher = new Regex(@"({(.)*})|(\[(.)*\])");
            List<TestTheoryJson> testConfiguration = new List<TestTheoryJson>();

            foreach (var item in deserialized.Properties().Where(t => t.Name == testMethod.Name))
            {
                Console.WriteLine(item);

                var tests = item.Value.ToObject<JObject>().Properties();

                foreach (var test in tests)
                {
                    var testName = test.Name;
                    var testContents = JObject.Parse(test.Value.ToString());
                    var theory = new TestTheoryJson();

                    theory.TestName = testName;
                    theory.inputs = BuildObject(testContents, JsonInputsProperty);
                    theory.outputs = BuildObject(testContents, JsonOutputsProperty);

                    testConfiguration.Add(theory);
                }
            }

            return BuildTestDataObjects(testConfiguration, testMethod).ToArray();
        }

        /// <summary>
        /// Converts the TestTheoryJSON test representations into consumable parameters for the test.
        /// </summary>
        /// <param name="testConfigurations">List of tests to extract.</param>
        /// <param name="testMethod">Method the test is for.</param>
        /// <returns></returns>
        private IEnumerable<object[]> BuildTestDataObjects(List<TestTheoryJson> testConfigurations, MethodInfo testMethod)
        {
            var testObjects = new List<object[]>();


            testConfigurations.ForEach(t =>
            {
                if (t.InspectForValidConfiguration(testMethod))
                {
                    testObjects.Add(t.ToObjectArray());
                }
                else
                {
                    throw new Exception($"{testMethod.Name}: Test configuration invalid.");
                }

            });

            return testObjects;
        }

        /// <summary>
        /// Extracts each of the elements to build either the inputs or the extpected values for the test into the Test Theory <see cref="TestTheoryJson"/> object.
        /// </summary>
        /// <param name="testContents"></param>
        /// <param name="jsonPropertyToBuild"></param>
        /// <returns></returns>
        private static object BuildObject(JObject testContents, string jsonPropertyToBuild)
        {
            var inputs = testContents.Properties().Single(t => t.Name == jsonPropertyToBuild).Value;
            return ExtractTestPropertyJSON(inputs);
        }

        /// <summary>
        /// Extracts the JSON test property from the JSON data.  A JSON test property (i.e. either inputs or outputs) can be either an array of objects or a single json object.
        /// </summary>
        /// <param name="testProperty">The specific property of the test</param>
        /// <returns>either a string or <see cref="JArray"/></cref> of strings</returns>
        private static object ExtractTestPropertyJSON(JToken testProperty)
        {
            if (Regex.IsMatch(testProperty.ToString().Replace("\r\n", ""), @"({(.)*})|(\[(.)*\])", RegexOptions.Multiline))
            {
                return JArray.Parse(testProperty.ToString()).ToObject<string[]>();
            }
            else
            {
                return testProperty.Value<string>();
            }
        }
    }
}
