﻿using EasilyNET.Core.Misc;

namespace EasilyNET.Test.Unit.Randoms;

[TestClass]
public class RandomTest
{
    private readonly Random _random = new();

    [TestMethod]
    public void StrictNext_ShouldReturnValueWithinRange()
    {
        // Act
        var result = _random.StrictNext();

        // Assert
        Assert.IsTrue(result is >= 0 and < int.MaxValue);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void StrictNext2_ShouldThrowArgumentOutOfRangeException_WhenStartIndexIsGreaterThanOrEqualToMaxValue()
    {
        // Act
        _random.StrictNext(10, 5);
    }
}