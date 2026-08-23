namespace PlcAiot.Abstractions.Faults;

/// <summary>故障解决状态机：Open → Acknowledged → InProgress → Resolved → Closed（或 AutoResolved）。</summary>
public enum FaultStatus { Open, Acknowledged, InProgress, Resolved, Closed, AutoResolved }
