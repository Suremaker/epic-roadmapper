﻿using System.Collections.Generic;
using System.Linq;

namespace JiraEpicRoadmapper.UI.Models
{
    public class ProjectLayout
    {
        public string Name { get; }
        public int ProjectRowIndex { get; }
        public IReadOnlyList<EpicVisualBlock> Epics { get; }
        public int LastRowIndex { get; }

        private ProjectLayout(string name, IReadOnlyList<EpicVisualBlock> epics, int projectRowIndex, int lastRowIndex)
        {
            Name = name;
            Epics = epics;
            ProjectRowIndex = projectRowIndex;
            LastRowIndex = lastRowIndex;
        }

        public static ProjectLayout Create(
            string name, IEnumerable<EpicMetadata> epics,
            int projectRowIndex, ILayoutDesigner designer)
        {
            var rows = designer.Layout(epics);
            var blocks = rows
                .SelectMany((row, index) => row.Select(m => new EpicVisualBlock(m, index + projectRowIndex + 1)))
                .ToArray();
            return new ProjectLayout(name, blocks, projectRowIndex, projectRowIndex + rows.Count);
        }
    }
}