namespace OptimizedFlange.Configuration

/// <summary>Defines user-facing unit-system preferences without changing stored project engineering values.</summary>
type DisplayUnitSystem =
    | SI
    | USCustomary

/// <summary>Defines application behavior that must remain separate from engineering calculation configuration.</summary>
type ApplicationSettings =
    {
        /// <summary>Schema version of this settings document.</summary>
        SchemaVersion: int
        /// <summary>Preferred UI language tag.</summary>
        Language: string
        /// <summary>Default display unit system for newly created projects.</summary>
        DefaultUnitSystem: DisplayUnitSystem
        /// <summary>Culture used by numeric input and presentation.</summary>
        NumericCulture: string
        /// <summary>Whether autosave is enabled.</summary>
        AutoSaveEnabled: bool
        /// <summary>Autosave interval in minutes.</summary>
        AutoSaveIntervalMinutes: int
        /// <summary>Default project save folder.</summary>
        DefaultProjectFolder: string
        /// <summary>Default report output folder.</summary>
        DefaultReportFolder: string
        /// <summary>Default backup folder.</summary>
        DefaultBackupFolder: string
    }
