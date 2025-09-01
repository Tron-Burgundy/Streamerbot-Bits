/**
*   Allows interruption of a delay by setting a global value that will intermittently checked for.
*
*   Arguments to configure the delay before calling it:

I_DELAY_TIME: in seconds.  e.g. 23.8
I_DELAY_NAME: e.g. shoutouts.  Setting a global temp value with this name will trigger the cancellation of this delay.
I_DELAY_INTERVAL: in seconds.  e.g. 0.25 to test for a break every quarter second
I_DELAY_AUTO_RESET: true or false.  Default is true.  Clears the variable that caused the break condition.

To interrupt a delay set a non-persisted global value to the value set in I_DELAY_NAME and set it to the reason for the break.

e.g. TEST_DELAY to "Shoutout skipped"

*/
using System;
using Newtonsoft.Json;

public class CPHInline
{
    int wssIndex;
	const string SOCKET_NAME = "console.log";
    const double MIN_INTERVAL = 0.1;
    const double MAX_INTERVAL = 2.0;
    const double DFLT_INTERVAL = 0.5;
    const string DFLT_DELAY_NAME = "I_DELAY_DEFAULT_NAME";

    string delayName = DFLT_DELAY_NAME;   // global var called I_DELAY_default would be checked

	public bool Execute()
	{
logc("Delay interruptable begins...", "c");

        try {
            if (!args.ContainsKey("I_DELAY_TIME")) {
                logc("ERROR: the argument I_DELAY_TIME has not been set");
                return false;
            }
logc("oh then");
            double delayTimeTotal = double.Parse(args["I_DELAY_TIME"].ToString());
            double interval = DFLT_INTERVAL;


            if (args.ContainsKey("I_DELAY_INTERVAL")) {
                var iVal = double.Parse(args["I_DELAY_INTERVAL"].ToString());

                if (iVal >= MIN_INTERVAL) interval = iVal;
            }

            if (args.ContainsKey("I_DELAY_NAME")) {
                delayName = args["I_DELAY_NAME"].ToString();
            }

logc("Delay: " + delayTimeTotal + " Interval: " + interval + " name: " + delayName);

            // if the delay time less than the interval is doesn't matter

            //int delayPerIntervalMS = interval * 1000;
            double delayLeft = delayTimeTotal;
            int intervalMS = (int)(interval * 1000.0);

                // do the delay

            while (delayLeft > interval) {
                //logc("Delay left:" + delayLeft, "y");

                if ( break_check() ) {
                    auto_reset_check();
                    return true;
                }

                CPH.Wait(intervalMS);

                delayLeft -= interval;
            }

            if ( break_check() ) {
                auto_reset_check();
                return true;
            }

logc("Delay remaining: " + delayLeft, "w");
                // wait out the remainder
            CPH.Wait((int)(delayLeft * 1000));

            auto_reset_check();

logc("Delay finished completely.", "g");
        } catch (Exception e) {
            logc("ERROR: " + e.ToString());
            log_dump(e);
        }

        return true;
    }

        // does a global temp with our name exist?

    public bool break_check() {
        var breakReason = CPH.GetGlobalVar<string>(delayName, false);  // non-persisted

        if (breakReason == null || breakReason.ToLower() == "false" || breakReason == "") {
            return false;
        }

logc("BREAK HAPPENED! - reason: " + breakReason, "m");

        // set an argument to check in the action history
        CPH.SetArgument("I_DELAY_INTERRUPT_REASON: ", breakReason);
        return true;
    }

    public void auto_reset_check() {
        if ( args.ContainsKey("I_DELAY_AUTO_RESET") ) {
            bool autoReset = false;
            string autoResetStr = args["I_DELAY_AUTO_RESET"].ToString();

            try {
                autoReset = Convert.ToBoolean(autoResetStr);
            } catch (Exception e) {
                logc("ERROR: I_DELAY_AUTO_RESET isn't a bool.  It's: '" + autoResetStr + "'", "r");
                return;
            }

            if (autoReset == false)
                return;
        }

            // remove the global temp

        CPH.SetGlobalVar(delayName, "", false);
logc("Reset delay interrupt trigger to null for " + delayName, "g");
    }

        // dummy versions when not using console logging.
    public void logc(string m = "", string c = "") {
    }
    public void log_dump(object o) {
    }

    ///
    //////////////// LOGGING WITH SOCKETS ////////////////////
    ///

/*         // log a string with a colour
	public void logc(string message, string colour = "w") {
		string outs = JsonConvert.SerializeObject( new {action="consolelog", message, colour} );
        CPH.WebsocketCustomServerBroadcast(outs, null, wssIndex);
	}
    public void Init() {
        wssIndex = CPH.WebsocketCustomServerGetConnectionByName(SOCKET_NAME);
    }
        // clears the browser's javascript console
	public void log_clear() {
		// CPH.SetArgument("logcommand", "clearconsole");
		// CPH.RunActionById("84fc51e6-9c97-48c9-8a66-23ba2460f7a4", true);
        string outs = JsonConvert.SerializeObject( new {action="consoleclear"} );
        CPH.WebsocketCustomServerBroadcast(outs, null, wssIndex);
	}
        // dump an object
    public void log_dump(object data) {
        string outs = JsonConvert.SerializeObject( new {action="dump", data} );
        CPH.WebsocketCustomServerBroadcast(outs, null, wssIndex);
    } */
}
