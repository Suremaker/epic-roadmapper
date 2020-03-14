﻿using System.Collections.Generic;
using System.Linq;

namespace JiraEpicRoadmapper.Client.Model
{
    public class ProjectLayout
    {
        public string Name { get; }
        public int ProjectRowIndex { get; }
        public bool ShowDependencies { get; }
        public IReadOnlyList<IReadOnlyList<EpicBlock>> Rows { get; }
        public int LastRowIndex => ProjectRowIndex + Rows.Count;

        public ProjectLayout(string name, int projectRowIndex, IEnumerable<EpicBlock> blocks, bool showDependencies)
        {
            Name = name;
            ProjectRowIndex = projectRowIndex;
            ShowDependencies = showDependencies;
            Rows = LayoutEpics(blocks);
        }

        private IReadOnlyList<IReadOnlyList<EpicBlock>> LayoutEpics(IEnumerable<EpicBlock> blocks)
        {
            var remaining = blocks.ToDictionary(e => e.Id, e => e.ResetRow());
            var lanes = new List<LinkedList<EpicBlock>>();
            while (remaining.Count > 0)
            {
                var epic = remaining.Values.OrderBy(r => r.NotLayoutParents).ThenBy(r => r.StartIndex).ThenByDescending(r => r.InboundRefs).ThenByDescending(r => r.Depth).First();
                var start = ShowDependencies ? epic.Inbounds.Select(x => x.Row - ProjectRowIndex - 1).Where(r => r >= 0).DefaultIfEmpty(0).Max() : 0;
                Allocate(epic, start);
            }

            void Allocate(EpicBlock e, int start = 0)
            {
                if (!remaining.Remove(e.Id))
                    return;

                var laneId = -1;
                for (var row = start; row < lanes.Count; row++)
                {
                    var lane = lanes[row];
                    var last = lane.Last.Value;

                    if ((!ShowDependencies || e.DependsOn(last) || !last.Dependents.Any()) && (e.StartIndex >= last.EndIndex))
                    {
                        laneId = row;
                        break;
                    }
                }

                if (laneId == -1)
                {
                    lanes.Add(new LinkedList<EpicBlock>());
                    laneId = lanes.Count - 1;
                }

                e.Row = ProjectRowIndex + laneId + 1;
                lanes[laneId].AddLast(e);
            }

            return lanes.Select(l => l.ToArray()).ToArray();
        }
    }
}