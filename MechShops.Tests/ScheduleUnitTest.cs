using FluentAssertions;
using MechShops.Domain.Shops;
using System;
using System.Linq;
using Xunit;

namespace MechShops.Tests;

public class ScheduleUnitTest
{
    [Fact]
    public void CreateSchedule_WithValidParameters_ResultObjectValidState()
    {
        Schedule obj = new Schedule(DateTime.Now, Guid.Empty.ToString(), 10, 0, 5);
        obj.IsValid.Should()
            .Be(true);
    }

    [Fact]
    public void CreateSchedule_InsufficientWorkLoad_NotificationsWorkLoad()
    {
        Schedule obj = new Schedule(DateTime.Now, Guid.Empty.ToString(), 10, 10, 12);
        obj.IsValid.Should()
            .Be(false);
        obj.Notifications.ElementAt(0).Key.Should()
            .Be("WorkLoad");
    }

    [Fact]
    public void CreateSchedule_IsWeekend_NotificationsWeekend()
    {
        Schedule obj = new Schedule(new DateTime(2029, 1, 7), Guid.Empty.ToString(), 10, 0, 5);
        obj.IsValid.Should()
            .Be(false);
        obj.Notifications.ElementAt(0).Message.Should()
            .Be("Cannot schedule for weekend");
    }

    [Fact]
    public void CreateSchedule_PastDate_NotificationsPastDate()
    {
        Schedule obj = new Schedule(DateTime.Now.AddDays(-1), Guid.Empty.ToString(), 10, 0, 5);
        obj.IsValid.Should()
            .Be(false);
        obj.Notifications.ElementAt(0).Message.Should()
            .Be("Cannot schedule for past dates");
    }
}
