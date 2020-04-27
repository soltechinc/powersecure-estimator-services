using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PowerSecure.Estimator.Services {
    public static class JTokenExtension {

        private static string IncrementString(string value) {
            var prefix = Regex.Match(value, "^\\D+").Value;
            var number = Regex.Replace(value, "^\\D+", "");
            var i = int.Parse(number) + 1;
            var newString = prefix + i.ToString(new string('0', number.Length));
            return newString;
        }

        private static string ChangeStringToZero(string value) {
            var prefix = Regex.Match(value, "^\\D+").Value;
            var number = Regex.Replace(value, "^\\D+", "");
            var i = int.Parse(number);
            var zero = 0;
            i = zero;
            var newString = prefix + i.ToString(new string('0', number.Length));
            return newString;
        }

        private static JToken SetTime(DateTime date) {
            date = DateTime.Today;
            return date.GetDateTimeFormats('d')[0];
        }

        public static dynamic WalkNode(this JToken node, string path) {
            switch (node.Type) {
                case JTokenType.Object:
                    foreach (var child in node.Children<JProperty>()) {
                        string childValue = child.Value.ToString();
                        string childName = child.Name.ToLower().ToString();
                        bool isPath = childName.Contains(path);
                        bool hasNum = childValue.Any(char.IsDigit);
                        DateTime date;
                        bool isDate = DateTime.TryParse(childValue, out date);
                        switch (path) {
                            case "revision":
                                if (isPath && hasNum) {
                                    if (isDate) {
                                        child.Value = SetTime(date);
                                    } else {
                                        child.Value = IncrementString(childValue);
                                    }
                                }
                                break;
                            case "version":
                                if (isPath && hasNum) {
                                    child.Value = IncrementString(childValue);
                                } else if(childName.Contains("revision")) {
                                    if (isDate) {
                                        child.Value = SetTime(date);
                                    } else {
                                        child.Value = ChangeStringToZero(childValue);
                                    }
                                }
                                break;
                        }
                        child.WalkNode(path);
                    }
                    break;
            }
            return node;
        }
    }
}
