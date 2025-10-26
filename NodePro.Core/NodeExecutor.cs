using NodePro.Abstractions.Attrs;
using NodePro.Abstractions.Enums;
using NodePro.Abstractions.Exceptions;
using NodePro.Abstractions.Interfaces;
using NodePro.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodePro.Core
{

    [NodeService(typeof(INodeExecutor))]

    public class NodeExecutor:INodeExecutor
    {
        private struct NodeInfo
        {
            public INodeContainer Node;
            public INodeContainer[] Input;
            public INodeContainer[] Output;
            public readonly int InputCount => Input.Length;
            public readonly int OutputCount => Output.Length;
        }

        private struct NodeConnectionInfo
        {
            public INodeContainer Node;
            public LinePair[] Input;
            public LinePair[] Output;
        }
        private class ExecutionGroup
        {
            private readonly INodeContainer[] _nodes = [];
            private readonly NodeInfo[] _infos = [];
            private readonly Dictionary<INodeContainer, NodeConnectionInfo> _infoMaps = [];
            private readonly Dictionary<INodeContainer, LinePair[]> _forwards = [];
            private readonly Dictionary<INodeContainer, LinePair[]> _backwards = [];

            public NodeInfo[] Infos => _infos;

            public NodeInfo this[int index]
            {
                get=> _infos[index];
            }

            public NodeConnectionInfo this[INodeContainer container]
            {
                get => _infoMaps.GetValueOrDefault(container);
            }
            public ExecutionGroup(INodeContainer[] nodes, List<LinePair> pairs)
            {
                _nodes = nodes;
                _forwards = pairs.GroupBy(x => x.Source.Node).ToDictionary(x => x.Key, g => g.ToArray());
                _backwards = pairs.GroupBy(x => x.Target.Node).ToDictionary(x => x.Key, g => g.ToArray());
                _infos = GetInfos();
                _infoMaps = nodes.Select(x =>
                {
                    return new NodeConnectionInfo()
                    {
                        Node = x,
                        Input = _backwards.GetValueOrDefault(x) ?? [],
                        Output=_forwards.GetValueOrDefault(x) ?? [],

                    };
                }).ToDictionary(x => x.Node);
            }

            private NodeInfo[] GetInfos()
            {
                return _nodes.Select(x =>
                new NodeInfo()
                {
                    Node = x,
                    Input = GetBackwards(x),
                    Output = GetForwards(x)
                }).ToArray();
            }

            public INodeContainer[] GetForwards()
            {
                return _infos.Where(x => x.InputCount == 0).Select(x => x.Node).ToArray();
            }

            public INodeContainer[] GetForwards(INodeContainer container)
            {
                return _forwards.GetValueOrDefault(container)?.Select(x => x.Target.Node).ToArray() ?? [];
            }

            public INodeContainer[] GetBackwards()
            {
                return _infos.Where(x => x.OutputCount == 0).Select(x => x.Node).ToArray();
            }

            public INodeContainer[] GetBackwards(INodeContainer container)
            {
                return _backwards.GetValueOrDefault(container)?.Select(x => x.Source.Node).ToArray() ?? [];
            }


        }

        private readonly INodeFormatService _formatter;

        public NodeExecutor(INodeFormatService formatService)
        {
            _formatter = formatService;
        }

        /// <summary>
        /// 流程: 
        /// 检查循环边 
        /// -> 寻找无输入节点(存在多个则选择第一个)
        /// -> 构建连通图
        /// 
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public NodeExecuteResult Execute(INodeContainer[] nodes, List<LinePair> pairs)
        {
            ExecutionGroup group = new(nodes, pairs);
            NodeExecuteResult res = NodeExecuteResult.INVALID;
            res = TopologicalSort(group, out List<INodeContainer> orderedNodes);
            if (orderedNodes.Count == 0) return NodeExecuteResult.NO_NODE;
            NodeExecutionData data = new NodeExecutionData();
            foreach (var node in orderedNodes) 
            {
                var info = group[node];
                node.Execute(data);
            }
            return res;
        }

        /// <summary>
        /// 对 ExecutionGroup 中的节点执行 Kahn 算法，返回拓扑排序结果
        /// （确保每个节点的所有输入节点（前驱）都先于它被排序）
        /// </summary>
        /// <param name="group">待排序的 ExecutionGroup 实例</param>
        /// <returns>拓扑排序后的节点列表</returns>
        /// <exception cref="InvalidOperationException">当存在环时抛出</exception>
        private NodeExecuteResult TopologicalSort(ExecutionGroup group,out List<INodeContainer> result)
        {
            // 1. 收集所有节点并建立节点到 NodeInfo 的映射（优化查找效率）
            var nodeToInfo = group.Infos.ToDictionary(info => info.Node);
            var allNodes = nodeToInfo.Keys.ToList();
            int totalNodeCount = allNodes.Count;

            // 2. 计算每个节点的初始入度（入度 = 输入节点数量，即 InputCount）
            var inDegree = new Dictionary<INodeContainer, int>();
            foreach (var node in allNodes)
            {
                inDegree[node] = nodeToInfo[node].InputCount;
            }

            // 3. 初始化队列：入度为 0 的节点（无前置依赖，可优先执行）
            var queue = new Queue<INodeContainer>();
            foreach (var node in allNodes)
            {
                if (inDegree[node] == 0)
                {
                    queue.Enqueue(node);
                }
            }

            // 4. 执行 Kahn 算法核心逻辑
            result = new List<INodeContainer>();
            while (queue.Count > 0)
            {
                // 取出当前入度为 0 的节点并加入结果
                var currentNode = queue.Dequeue();
                result.Add(currentNode);

                // 获取当前节点的所有输出节点（后继节点）
                var successors = group.GetForwards(currentNode);

                // 遍历后继节点，减少其入度（当前节点已处理，解除一个前置依赖）
                foreach (var successor in successors)
                {
                    inDegree[successor]--;

                    // 若后继节点入度变为 0，说明其所有前置依赖已处理，加入队列
                    if (inDegree[successor] == 0)
                    {
                        queue.Enqueue(successor);
                    }
                }
            }

            // 5. 检测环：若结果中节点数量不等于总节点数，说明存在环
            if (result.Count != totalNodeCount)
            {
                return NodeExecuteResult.LOOP_DETECTED;
            }

            return NodeExecuteResult.SUCCESS;
        }

    }
}
