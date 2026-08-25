namespace PlcVsa.Contracts.Faults;

/// <summary>故障严重度（顺序 Info &lt; Warning &lt; Error &lt; Critical，便于阈值过滤）。</summary>
public enum FaultSeverity { Info, Warning, Error, Critical }
