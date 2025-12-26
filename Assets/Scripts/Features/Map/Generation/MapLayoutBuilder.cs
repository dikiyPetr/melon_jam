using System.Collections.Generic;
using UnityEngine;

public class MapLayoutBuilder
{
    private MapConfig _config;
    private Dictionary<int, MapRowData> _rows;
    private int _currentRow;
    private MapConnectionBuilder _connectionBuilder;

    public MapLayoutBuilder(MapConfig config)
    {
        _config = config;
        _rows = new Dictionary<int, MapRowData>();
        _currentRow = 0;
        _connectionBuilder = new MapConnectionBuilder(config);
    }

    public Dictionary<int, MapRowData> Build()
    {
        _rows.Clear();
        _currentRow = 0;

        foreach (var lineConfig in _config.MapLines)
        {
            CreateLine(lineConfig);
        }

        return _rows;
    }

    private void CreateLine(MapLineConfig lineConfig)
    {
        List<MapNodeData> previousRow = GetNodesInRow(_currentRow - 1);
        MapRowType rowType = GetRowTypeFromLineType(lineConfig.LineType);

        var rowData = new MapRowData(_currentRow, rowType);

        int startColumn = lineConfig.NodeCount == 1 ? _config.ColumnsPerRow / 2 : 0;

        for (int i = 0; i < lineConfig.NodeCount; i++)
        {
            int column = lineConfig.NodeCount == 1 ? startColumn : i;
            MapNodeType nodeType = GetNodeTypeForLine(lineConfig.LineType);

            var node = new MapNodeData(
                GenerateNodeId(column, _currentRow),
                nodeType,
                new Vector2Int(column, _currentRow)
            );

            rowData.AddNode(column, node);
        }

        _rows[_currentRow] = rowData;
        List<MapNodeData> currentRowNodes = rowData.GetAllNodes();

        if (previousRow.Count > 0)
        {
            if (lineConfig.NodeCount == 1)
            {
                _connectionBuilder.ConnectToSingleNode(previousRow, currentRowNodes[0]);
            }
            else if (lineConfig.HasGuaranteedConnections)
            {
                _connectionBuilder.ConnectCheckpointLine(previousRow, currentRowNodes);
            }
            else
            {
                _connectionBuilder.ConnectRows(previousRow, currentRowNodes);
            }
        }

        _currentRow++;
    }

    private MapRowType GetRowTypeFromLineType(MapLineType lineType)
    {
        switch (lineType)
        {
            case MapLineType.Start:
                return MapRowType.Start;
            case MapLineType.Boss:
                return MapRowType.Boss;
            case MapLineType.CheckpointLine:
            case MapLineType.RestLine:
                return MapRowType.Checkpoint;
            case MapLineType.Default:
            default:
                return MapRowType.Default;
        }
    }

    private MapNodeType GetNodeTypeForLine(MapLineType lineType)
    {
        switch (lineType)
        {
            case MapLineType.Start:
                return MapNodeType.Start;
            case MapLineType.Boss:
                return MapNodeType.Boss;
            case MapLineType.RestLine:
                return MapNodeType.Rest;
            case MapLineType.CheckpointLine:
                // todo добавить выбор из пула
                return MapNodeType.Shop;
            case MapLineType.Default:
            default:
                return MapNodeType.Unknown;
        }
    }

    private List<MapNodeData> GetAllNodes()
    {
        List<MapNodeData> allNodes = new List<MapNodeData>();
        foreach (var row in _rows.Values)
        {
            allNodes.AddRange(row.GetAllNodes());
        }

        return allNodes;
    }

    private List<MapNodeData> GetNodesInRow(int rowIndex)
    {
        if (_rows.ContainsKey(rowIndex))
        {
            return _rows[rowIndex].GetAllNodes();
        }

        return new List<MapNodeData>();
    }

    private string GenerateNodeId(int col, int row)
    {
        return $"node_{row}_{col}";
    }
}