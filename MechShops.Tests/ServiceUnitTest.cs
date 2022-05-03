using FluentAssertions;
using MechShops.Domain.Shops;
using System;
using System.Linq;
using Xunit;

namespace MechShops.Tests;

public class ServiceUnitTest
{
    [Fact]
    public void CreateService_WithValidParameters_ResultObjectValidState()
    {
        Service obj = new Service("Test service", 5, Guid.Empty.ToString());
        obj.IsValid.Should()
            .Be(true);
    }

    [Fact]
    public void CreateService_InvalidName_NotificationsName()
    {
        Service obj = new Service("", 2, Guid.Empty.ToString());
        obj.IsValid.Should()
            .Be(false);
        obj.Notifications.ElementAt(0).Key.Should()
            .Be("Name");
    }

    [Fact]
    public void CreateService_InvalidWorkUnits_NotificationsWorkUnits()
    {
        Service obj = new Service("Test service", 0, Guid.Empty.ToString());
        obj.IsValid.Should()
            .Be(false);
        obj.Notifications.ElementAt(0).Key.Should()
            .Be("WorkUnits");
    }
}
