using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AttachedTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void AttachedTestSimplePasses()
    {
        Attachment attach = new Attachment();
        GameObject arm = new GameObject("Robot_Upperarm_Left");
        attach.SetAttachmentObject = arm;
        Assert.AreEqual(true, attach.CheckName("Robot_Upperarm_Left"));
    }

}
