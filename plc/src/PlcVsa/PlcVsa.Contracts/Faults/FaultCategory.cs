namespace PlcVsa.Contracts.Faults;

/// <summary>故障分类，用于聚合统计与路由恢复策略。</summary>
public enum FaultCategory { Comm, Protocol, Safety, Rule, System, Device }
