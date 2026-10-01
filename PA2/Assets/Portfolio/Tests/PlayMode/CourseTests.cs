using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using PA2.Portfolio;

public class CourseTests
{
    RunSession run;
    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("RelayRun");
        yield return null;
        yield return null;
        run=RunSession.Instance;
        Assert.NotNull(run);
        run.SilentTest=true;
        run.player.TestInput=true;
        run.Begin();
    }
    [UnityTearDown]
    public IEnumerator Cleanup() { Time.timeScale=1; yield return null; }

    [UnityTest]
    public IEnumerator MovementDashDamageAndPause()
    {
        Vector3 start=run.player.transform.position;
        run.player.SimulatedInput=Vector2.up;
        yield return new WaitForSeconds(.35f);
        Assert.Greater(run.player.transform.position.z,start.z+1);
        run.player.SimulatedDash=true;
        yield return null;
        yield return null;
        Assert.IsTrue(run.player.IsDashing);
        Assert.IsFalse(run.Damage(20),"Dash must evade damage");
        run.player.SimulatedInput=Vector2.zero;
        yield return new WaitForSeconds(.3f);
        Assert.Less(run.player.DashReady,1);
        Assert.IsTrue(run.Damage(20));
        Assert.IsFalse(run.Damage(20),"Grace period blocks duplicate collision damage");
        Assert.AreEqual(80,run.Health);
        run.Pause();
        float remaining=run.Remaining;
        yield return new WaitForSecondsRealtime(.15f);
        Assert.AreEqual(remaining,run.Remaining);
        run.Resume();
        Assert.AreEqual(RunState.Playing,run.State);
    }

    [UnityTest]
    public IEnumerator PhysicsTriggersCheckpointHealingAndVictory()
    {
        Assert.IsFalse(run.ActivateRelay(1,new Vector3(0,.1f,66)),"Relays must activate in order");
        run.Damage(20);
        run.player.Teleport(new Vector3(0,.1f,31));
        yield return new WaitForSeconds(.2f);
        Assert.AreEqual(1,run.Relays);
        run.player.Teleport(new Vector3(-5.8f,.1f,20));
        yield return new WaitForSeconds(.2f);
        Assert.AreEqual(1,run.Cells);
        Assert.AreEqual(100,run.Health);
        run.player.Teleport(new Vector3(0,-6,35));
        yield return new WaitForSeconds(.2f);
        Assert.Less(Vector3.Distance(run.player.transform.position,run.Checkpoint),1);
        run.TryExit();
        Assert.AreEqual(RunState.Playing,run.State);
        run.player.Teleport(new Vector3(0,.1f,65));
        yield return new WaitForSeconds(.2f);
        Assert.AreEqual(2,run.Relays);
        run.player.Teleport(new Vector3(0,.1f,97));
        yield return new WaitForSeconds(.2f);
        Assert.AreEqual(3,run.Relays);
        run.player.Teleport(new Vector3(0,.1f,106));
        yield return new WaitForSeconds(.2f);
        Assert.AreEqual(RunState.Won,run.State);
    }

    [UnityTest]
    public IEnumerator DamageFailureStopsSessionAndRetryResets()
    {
        Assert.IsTrue(run.Damage(100));
        Assert.AreEqual(RunState.Lost,run.State);
        float remaining=run.Remaining;
        yield return new WaitForSeconds(.1f);
        Assert.AreEqual(remaining,run.Remaining);
        Assert.IsFalse(run.CollectCell());
        run.Retry();
        yield return null;
        yield return null;
        Assert.AreEqual(RunState.Menu,RunSession.Instance.State);
        Assert.AreEqual(100,RunSession.Instance.Health);
        Assert.AreEqual(0,RunSession.Instance.Relays);
    }
}
