#if UNITY_EDITOR && UNITY_INCLUDE_TESTS
using NUnit.Framework;
using System.Linq;
namespace VuonVietKyThu.Tests {
    public class LevelDifficultyTests {
        LevelDatabase db;
        [SetUp] public void Setup()=>db=new LevelDatabase();
        [Test] public void HasExactly50Levels()=>Assert.AreEqual(50,db.Data.levels.Length);
        [Test] public void FirstTenAreOnboardingEasy(){
            var first=db.Data.levels.Take(10).ToArray();Assert.IsTrue(first.All(l=>l.moves>=28));Assert.IsTrue(first.All(l=>l.typeCount<=5));Assert.LessOrEqual(first.Max(l=>l.jelly),14);Assert.AreEqual(1,first[0].goals.Count);Assert.AreEqual(1,first[1].goals.Count);Assert.IsTrue(first.All(l=>l.goalBias>=.24f));
        }
        [Test] public void DifficultyStartsRisingAfterTen(){Assert.AreEqual(6,db.Get(11).typeCount);Assert.Greater(db.Get(20).goals.Sum(g=>g.count),db.Get(3).goals.Sum(g=>g.count));}
    }
}
#endif
