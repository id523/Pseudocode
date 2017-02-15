using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PseudocodeRevisited.FileAccess {
    public static class File {
        public static FileStream Open(string path, FileMode fileMode) {
            try {
                return new FileStream(path, fileMode);
            } catch (IOException ex) {
                throw new RuntimeException("Unable to open the file " + path + ":\n" + ex.Message);
            } catch (System.Security.SecurityException ex) {
                throw new RuntimeException("Access denied:\n" + ex.Message);
            } catch (ArgumentException) {
                if (Enum.IsDefined(typeof(FileMode), fileMode)) {
                    throw new RuntimeException((path?.ToString() ?? "null") + " is not a valid file path.");
                } else {
                    throw new RuntimeException("The file-access mode specified is not valid.");
                }
            }
        }
        public static void Close(FileStream fs) {
            fs.Dispose();
        }
        public static void WriteText(FileStream fs, string text) {
            try {
                byte[] encoded = Encoding.UTF8.GetBytes(text);
                fs.Write(encoded, 0, encoded.Length);
            } catch (Exception ex) {
                throw new RuntimeException("Unable to write text to file:\n" + ex.Message);
            }
        }
    }
}
