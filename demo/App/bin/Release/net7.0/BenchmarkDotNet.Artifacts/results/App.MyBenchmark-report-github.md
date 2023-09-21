```

BenchmarkDotNet v0.13.8, Windows 11 (10.0.22621.2283/22H2/2022Update/SunValley2)
12th Gen Intel Core i7-1260P, 1 CPU, 16 logical and 12 physical cores
.NET SDK 8.0.100-rc.1.23463.5
  [Host] : .NET 7.0.11 (7.0.1123.42427), X64 RyuJIT AVX2

Job=MediumRun  Toolchain=InProcessNoEmitToolchain  IterationCount=15  
LaunchCount=2  WarmupCount=10  

```
| Method                          | Mean         | Error       | StdDev      | Gen0   | Allocated |
|-------------------------------- |-------------:|------------:|------------:|-------:|----------:|
| UseEntity                       | 6,277.788 ns | 206.2257 ns | 295.7627 ns | 0.2365 |    2240 B |
| UseEntityBufferedMessageForeach |     5.508 ns |   0.2896 ns |   0.4335 ns |      - |         - |
