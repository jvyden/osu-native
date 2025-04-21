using osu.Native.Bindings;

string beatmap = args[0];
int ruleset = int.Parse(args[1]);
uint mods = uint.Parse(args[2]);

Lazer lazer = new Lazer();
lazer.Log += msg => Console.WriteLine($"[Lazer] : {msg}");

using LazerBeatmap map = lazer.LoadDifficultyFromFile(new FileInfo(beatmap));
double sr = lazer.ComputeDifficulty(map, ruleset, mods);

Console.WriteLine(sr);
