﻿using System;
using System.Collections.Generic;
using System.Text;
using JiraEpicRoadmapper.Contracts;
using JiraEpicRoadmapper.UI.Models;
using Shouldly;
using Xunit;

namespace JiraEpicRoadmapper.UI.UnitTests
{
    public class StatusVisualizerTests
    {
        private readonly IStatusVisualizer _visualizer = new StatusVisualizer();

        [Theory]
        [InlineData("Done", "✔️")]
        [InlineData("In Progress", "🛠️")]
        [InlineData("To Do", "❔")]
        [InlineData("Something", "❔")]
        public void It_should_visualize_status_category(string statusCategory, string expected)
        {
            var block = new EpicVisualBlock(new EpicMetadata(new Epic { StatusCategory = statusCategory }, new IndexedDay(), new IndexedDay()), 1);
            _visualizer.GetStatusIcons(block).ShouldContain(expected);
        }
    }
}
