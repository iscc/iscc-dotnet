using System;
using System.Reflection;
using System.Linq;

namespace ISCCTests
{
    /// <summary>
    /// Storage container for translating the JSON contents of a test into a C# XUnit test.
    /// </summary>
    public class TestTheoryJson
    {
        public string TestName { get; set; }
        public object inputs { get; set; }
        public object outputs { get; set; }

        
        /// <summary>
        /// Verifies the number of inputs and expected outputs of the test match the method signature.
        /// </summary>
        /// <param name="method">Method that will be invoked with this test configuration.</param>
        /// <returns>Vlidity of the testmethod and this class's data.</returns>
        public bool InspectForValidConfiguration(MethodInfo method)
        {
            var inputCount = method.GetParameters().Count(t => t.Name.ToLower().Contains("input"));
            var outputCount = method.GetParameters().Count(t => t.Name.ToLower().Contains("expected")); 


            return InspectForValidConfiguration(inputs, inputCount) && InspectForValidConfiguration(outputs, outputCount);
        }
        
        /// <summary>
        /// Validates a group of inputs or outputs for count match.
        /// </summary>
        /// <param name="property">A class property either inputs or outputs.</param>
        /// <param name="propertyCount">Number of expected items in the property.</param>
        /// <returns>True if the counts match for the given property.</returns>
        private static bool InspectForValidConfiguration(object property, int propertyCount)
        {
            var valid = true;
            if(propertyCount == 0 && property != null) valid = false;
            else if(propertyCount == 1 && (property.GetType().IsArray && ((object[])property).Length != 1)) valid = false;
            else if(propertyCount > 1 && (!property.GetType().IsArray || ((object[])property).Length != propertyCount)) valid = false;
            return valid;
        }

        /// <summary>
        /// Conversion utility to convert this object to an object array.
        /// </summary>
        /// <returns></returns>
        public object[] ToObjectArray() => new[] { (string)TestName, inputs, outputs };
    }
}
