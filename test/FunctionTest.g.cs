// This code was generated by Hypar.
// Edits to this code will be overwritten the next time you run 'hypar init'.
// DO NOT EDIT THIS FILE.

using Xunit;
using Hypar.Functions.Execution;
using System.Threading.Tasks;
using Elements;
using Elements.Geometry;
using Xunit.Abstractions;
using Hypar.Functions.Execution.Local;

namespace FARLimitedMassing.Tests
{
    public class FunctionTests
    {
        private readonly ITestOutputHelper output;

        public FunctionTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task InvokeFunction()
        {
            var store = new FileModelStore<FARLimitedMassingInputs>("./",true);

            // Create an input object with default values.
            var input = new FARLimitedMassingInputs();

            // Invoke the function.
            // The function invocation uses a FileModelStore
            // which will write the resulting model to disk.
            // You'll find the model at "./model.gltf"
            var l = new InvocationWrapper<FARLimitedMassingInputs,FARLimitedMassingOutputs>(store, FARLimitedMassing.Execute);
            var output = await l.InvokeAsync(input);
        }
    }
}