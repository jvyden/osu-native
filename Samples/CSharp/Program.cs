using osu.Native.Bindings;

string beatmap = args[0];
int ruleset = int.Parse(args[1]);
uint mods = uint.Parse(args[2]);

Lazer lazer = new Lazer();
lazer.Log += msg => Console.WriteLine($"[Lazer] : {msg}");

using LazerBeatmap map = lazer.LoadDifficultyFromFile(new FileInfo(beatmap));
double sr = lazer.ComputeDifficulty(map, ruleset, mods);

Console.WriteLine(sr);

Console.WriteLine("Strain:");
Dictionary<string, double[]> strains = lazer.ComputeStrain(map, ruleset);
foreach (KeyValuePair<string, double[]> keyValuePair in strains)
{
    Console.WriteLine(keyValuePair.Key);
    foreach (double d in keyValuePair.Value)
    {
        Console.Write('\t');
        Console.WriteLine(d);
    }
}
