using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
/*
    On initialisation for each file it converts them to a list and shuffles them.
    All requests then work their way down the list until it repeats.

    set argument 'warning' to false to stop error output
*/


public class CPHInline
{
    public Dictionary<string, List<string>> lines = new Dictionary<string, List<string>>();
    public Dictionary<string, int> indexes = new Dictionary<string, int>();

    public const string FILE_ARG_NAME = "filename";
    public const string OUTPUT_NAME = "randomline";
    public const string ERROR_PREFIX = "[RANDOM LINE] error: ";

    public bool Execute()
    {
        if ( false == args.ContainsKey(FILE_ARG_NAME) ) {
            warn(ERROR_PREFIX + "No file to read set.  Set argument " + FILE_ARG_NAME + " to the file name to read lines from.");
            return false;
        }

        string filename = args[FILE_ARG_NAME].ToString();

        if (false == lines.ContainsKey(filename)) {
            if (!File.Exists(filename)) {
                warn(ERROR_PREFIX + "File does not exist: " + filename);
                return false;
            }

            setup_dictionary(filename);
        }

        string subbed = sub_args( get_rand_line(filename) );
        CPH.SetArgument(OUTPUT_NAME, subbed);
        //say(subbed);

        return true;
    }

        // returns the line, updates the counter

    public string get_rand_line(string key) {
        int idx = this.indexes[key];
        string line = this.lines[key][idx];
        this.indexes[key] = ++idx % this.lines[key].Count;
        return line;
    }

    public void warn(string msg) {
        bool suppressWarnings = args.ContainsKey("warning") ? args["warning"].ToString().ToLower() == "true" : false;
        if (suppressWarnings) return;
        say(msg);
    }

        // read the lines into the dictionary

    public void setup_dictionary(string key) {
        //say("Setting up dictionary for " + key);
        try {
            this.lines[key] = new List<string>(File.ReadAllLines(key));
            this.indexes[key] = 0;
            Shuffle(this.lines[key]);
        } catch (Exception e) {
            warn(ERROR_PREFIX + e.ToString());
        }
        //say("It is set up, number of lines: " + this.lines[key].Count);
    }

        // %replaces% into args

    public string sub_args(string msg) {
        foreach (var arg in args) {
            //msg = msg.Replace($"%{arg.Key}%", arg.Value.ToString());// case sensitive
            msg = Regex.Replace(msg, $"%{arg.Key}%", arg.Value.ToString(), RegexOptions.IgnoreCase);
        }
        return msg;
    }

    public void say(string w2s, bool bot = true) {
        CPH.SendMessage(w2s, bot);
    }

	public void Shuffle<T>(IList<T> list)
	{
		Random rng = new Random();

	    int n = list.Count;
	    while (n > 1) {
	        n--;
	        int k = rng.Next(n + 1);
	        T value = list[k];
	        list[k] = list[n];
	        list[n] = value;
	    }
	}

}
