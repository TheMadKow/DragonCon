using DragonCon.Logical.Convention;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.BDDfy;

namespace DragonCon.Specs.AsConventionManager
{
    [Story(AsA = "As Convention Manager",
        IWant = "To create new convention object",
        SoThat = "I can add convention information")]
    [TestClass]
    public class CanAddConvention
    {
        [TestMethod]
        public void AddConvention()
        {
            this.Given(_ => GivenUserIsConventionManager())
                .And(_ => GivenConventionBuilder())
                .And(_ => GivenConventionData())
                .When(_ => CreatingNewConvention())
                .Then(_ => ThenConventionDataIsSaved())
                .BDDfy<ConventionBuilder>();
        }
    }
}
