using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestPerception {

    [Test]
    public void TestPerceptionHerbivorousSeeFood() {
        // Use the Assert class to test conditions.
        /*
        var agentGo = new GameObject("HerbivorousAgent");
        var perception = agentGo.AddComponent<Perception>();

        var food = new GameObject("Herb") {tag = "food"};

        Assert.Equals(1f, perception.Perceive(100, new float[] {0}, new string[] {"food"}, 0, 0));
        */
    }
    
    


    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator TestPerceptionWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
