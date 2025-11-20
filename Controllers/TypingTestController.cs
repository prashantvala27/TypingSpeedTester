using System;
using System.Linq;
using System.Web.Mvc;
using TypingSpeedTester.DAL;
using TypingSpeedTester.Models;

namespace TypingSpeedTester.Controllers
{
    public class TypingTestController : Controller
    {
        private TypingTestDAL dal = new TypingTestDAL();

        // Array of sample texts for typing test
        private string[] sampleTexts = new string[]
        {
            "The quick brown fox jumps over the lazy dog. This pangram contains every letter of the English alphabet at least once.",
            "Practice makes perfect. The more you type, the better you become at typing faster and more accurately with fewer errors.",
            "Technology has revolutionized the way we communicate and work in the modern world. Computers are now essential tools.",
            "Learning to type without looking at the keyboard is an essential skill for productivity in today's digital workplace.",
            "Consistency and dedication are the keys to improving your typing speed over time. Regular practice yields great results.",
            "Programming requires fast and accurate typing skills. Developers spend most of their time writing code on computers.",
            "Touch typing allows you to focus on your thoughts rather than searching for keys on the keyboard constantly.",
            "The internet has connected billions of people worldwide. Communication happens instantly across vast distances today."
        };

        // GET: TypingTest/Index (Main test page)
        public ActionResult Index()
        {
            Random rand = new Random();
            string randomText = sampleTexts[rand.Next(sampleTexts.Length)];

            ViewBag.TestText = randomText;
            return View();
        }

        // POST: Save test result
        [HttpPost]
        public ActionResult SaveResult(string userName, string testText, string typedText,
                                      int timeDuration, int errorCount)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(typedText))
                {
                    TempData["Error"] = "Invalid input data. Please try again.";
                    return RedirectToAction("Index");
                }

                // Calculate WPM
                string[] words = typedText.Trim().Split(new char[] { ' ', '\n', '\r', '\t' },
                                StringSplitOptions.RemoveEmptyEntries);
                int wordsTyped = words.Length;
                double minutes = timeDuration / 60.0;
                int wpm = minutes > 0 ? (int)Math.Round(wordsTyped / minutes) : 0;

                // Calculate Accuracy
                int correctChars = 0;
                int minLength = Math.Min(testText.Length, typedText.Length);

                for (int i = 0; i < minLength; i++)
                {
                    if (testText[i] == typedText[i])
                        correctChars++;
                }

                int totalCharsToCheck = Math.Max(testText.Length, typedText.Length);
                decimal accuracy = totalCharsToCheck > 0 ?
                                 (decimal)correctChars / totalCharsToCheck * 100 : 0;

                // Create test object
                TypingTest test = new TypingTest
                {
                    UserName = userName.Trim(),
                    TestDate = DateTime.Now,
                    WPM = wpm,
                    Accuracy = Math.Round(accuracy, 2),
                    ErrorCount = errorCount,
                    TimeDuration = timeDuration,
                    TestText = testText,
                    TypedText = typedText
                };

                // Save to database
                bool success = dal.SaveTestResult(test);

                if (success)
                {
                    TempData["Message"] = "Test result saved successfully!";
                    TempData["WPM"] = wpm;
                    TempData["Accuracy"] = Math.Round(accuracy, 2);
                    TempData["ErrorCount"] = errorCount;
                    TempData["TimeDuration"] = timeDuration;
                    return RedirectToAction("Result");
                }
                else
                {
                    TempData["Error"] = "Failed to save result to database.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Show result page
        public ActionResult Result()
        {
            return View();
        }

        // GET: View all test history
        public ActionResult History()
        {
            var tests = dal.GetAllResults();
            return View(tests);
        }

        // GET: View details of a specific test
        public ActionResult Details(int id)
        {
            var test = dal.GetResultById(id);
            if (test == null)
            {
                TempData["Error"] = "Test result not found.";
                return RedirectToAction("History");
            }
            return View(test);
        }

        // GET: Delete a test result
        public ActionResult Delete(int id)
        {
            bool success = dal.DeleteResult(id);
            if (success)
            {
                TempData["Message"] = "Result deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete result.";
            }
            return RedirectToAction("History");
        }

        // GET: Statistics page
        public ActionResult Statistics(string userName = null)
        {
            var stats = dal.GetStatistics(userName);
            ViewBag.UserName = userName;
            return View(stats);
        }
    }
}