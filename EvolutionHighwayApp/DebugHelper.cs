using System.Diagnostics;
using System.Windows.Browser;

namespace EvolutionHighwayApp
{
    public class DebugHelper
    {
        private const string JsDebug = 
              @"function _dbg(text)
                {        // VS script debugger output window.
                        if ((typeof(Debug) !== 'undefined') && Debug.writeln) {
                            Debug.writeln(text);
                        }
                        // FF firebug and Safari console.
                        if (window.console && window.console.log) {
                            window.console.log(text);
                        }
                        // Opera console.
                        if (window.opera) {
                            window.opera.postError(text);
                        }
                        // WebDevHelper console.
                        if (window.debugService) {
                            window.debugService.trace(text);
                        }
                };";
        private const string JsDebugExec = "_dbg('{0}');";

        public static void Write(string text)
        {
            Debug.WriteLine(text);
            HtmlPage.Window.Eval(JsDebug + string.Format(JsDebugExec, (text ?? "").Replace("\\", "\\\\").Replace("'", "\\'")));
        }
    }
}
