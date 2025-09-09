# Verified Comparison: Datadog Logs vs. Datadog Events

The fundamental difference between Datadog Logs and Events is their intended purpose: **Logs** provide high-volume, detailed records for deep analysis and troubleshooting, while **Events** offer low-volume, high-signal notifications that mark important, discrete occurrences.

### Core Distinction

*   **Datadog Logs:** The full, detailed story of everything your application is doing. It's the verbose, raw evidence needed for deep troubleshooting. Think of it as the *firehose*.
*   **Datadog Events:** Concise, high-signal notifications about specific, important occurrences. They are the milestones in your application's story. Think of it as the *headline*.

---

### Detailed Comparison Table

| Aspect | Datadog Logs Management | Datadog Event Management (Custom Events) |
| :--- | :--- | :--- |
| **Primary Function** | **Troubleshooting & Deep Analysis.** Logs provide "granular, continuous records of activity and behavior" for "troubleshooting, auditing, [and] pattern detection." | **Correlation & Situational Awareness.** Events "capture discrete, significant occurrences" used for "correlating major incidents or changes to observed performance issues" and "overlaying key events on dashboards to provide context." |
| **Data Nature** | **High-Volume, High-Cardinality.** Described as "high in volume" and designed for "large volumes of operational data." | **Low-Volume, High-Signal.** Described as "lower in volume than logs" and meant to record "notable moments." |
| **Verified Use Cases** | • **Debugging & Troubleshooting:** Analyzing application/infrastructure issues.<br>• **Auditing & Security:** Monitoring system activity.<br>• **Analytics:** Long-term pattern recognition and root cause analysis.<br>• **Alerting:** Creating monitors from log patterns. | • **Tracking Changes:** Code deployments, configuration changes.<br>• **Incident Context:** Correlating events like monitor alerts or outages with performance data.<br>• **Dashboard Overlays:** Visualizing key changes directly on metric graphs. |
| **Cost Model** | Based on **GBs of data ingested and indexed**. This is the standard model for high-volume log management solutions. | Based on the **number of events** submitted. The cost per event is very low, encouraging their use for marking important milestones. |
| **Key Advantage** | **Deep, searchable history.** The ability to perform complex queries on a rich, detailed dataset to find the root cause of any issue. | **Instant correlation.** The ability to see events as overlays on every graph is their superpower, immediately connecting an action (like a deploy) to an outcome (like a spike in errors). |

---

### When to Use Which?

*   **Use Datadog Logs when you need to answer "Why did this happen?"**
    *   You have an error and need the full stack trace.
    *   A user reports a bug, and you need to see their exact actions.
    *   You need to analyze the performance of a specific transaction.

*   **Use Datadog Events when you need to answer "What was happening at this time?"**
    *   You see a metric spike and want to know if a deployment just occurred.
    *   You want a clear timeline of all production infrastructure changes.
    *   You need to record a manual action, like a database failover, for future reference.

### Synergy: Better Together

The true power comes from using them together. An **Event** tells you *when* to look, and the **Logs** tell you *what* to look for. For example, an event for a code deployment can be directly correlated with a new error appearing in the logs, drastically reducing the time to resolution.

# Example App

[Datadog.Api Example App](Datadog.Api/README.md)

# Resources

Example dotnet api app https://github.com/azure-samples/dotnet-core-api/tree/master/

Log Management, C# Log Collection https://docs.datadoghq.com/logs/log_collection/csharp/?tab=serilog
https://github.com/DataDog/serilog-sinks-datadog-logs/tree/master?tab=readme-ov-file#serilogsinksdatadoglogs
