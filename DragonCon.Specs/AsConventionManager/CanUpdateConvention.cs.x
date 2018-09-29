using DragonCon.Logical.Convention;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.BDDfy;

namespace DragonCon.Specs.AsConventionManager
{
    [Story(AsA = "As Convention Manager",
        IWant = "To update existing convention object",
        SoThat = "I can update convention information and fix mistakes")]
    [TestClass]
    public class CanUpdateConvention
    {
        [TestMethod]
        public void AddConventionTest()
        {
            this.Given(_ => GivenUserIsConventionManager())
                .And(_ => GivenConventionBuilder())
                .And(_ => GivenConventionID())
                .And(_ => GivenConventionUpdateData())
                .When(_ => UpdatingExisitngConvention())
                .Then(_ => ThenConventionDataIsSaved())
                .BDDfy<ConventionBuilder>();
        }
    }
}
