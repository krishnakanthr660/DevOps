using Microsoft.AspNetCore.Mvc;
using PostHog;

namespace PostHogTrial.Controllers
{
    public class DemoController : Controller
    {
        private readonly IPostHogClient _posthog;

        public DemoController(IPostHogClient posthog)
        {
            _posthog = posthog;
        }

        // ─────────────────────────────────────────────  
        // HOME – shows all demo sections  
        // ─────────────────────────────────────────────  
        public IActionResult Index()
        {
            return View();
        }

        // ─────────────────────────────────────────────  
        // 1. PRODUCT ANALYTICS – capture events + identify  
        // ─────────────────────────────────────────────  
        public async Task<IActionResult> Signup(string email)
        {
            var userId = Guid.NewGuid().ToString();

            // Identify the user (creates a person profile in PostHog)  
            await _posthog.IdentifyAsync(userId, new Dictionary<string, object>
            {
                ["email"] = email,
                ["name"] = "Demo User",
                ["plan"] = "free",
                ["signed_up_at"] = DateTime.UtcNow.ToString("o")
            }, null, CancellationToken.None);

            // Capture a custom event with properties  
            _posthog.Capture(userId, "user_signed_up", new Dictionary<string, object>
            {
                ["signup_method"] = "email",
                ["referrer"] = "demo_app"
            });

            // Track a page view from the backend  
            _posthog.CapturePageView(userId, "/signup");

            ViewBag.UserId = userId;
            ViewBag.Message = $"Signup event captured for {email}";
            return View("Result");
        }

        // ─────────────────────────────────────────────  
        // 2. FEATURE FLAGS – gate features per user  
        // ─────────────────────────────────────────────  
        public async Task<IActionResult> FeatureFlags(string userId)
        {
            // Simple boolean flag  
            var newDashboardEnabled = await _posthog.IsFeatureEnabledAsync(
                userId, "new-dashboard");

            // Multivariate flag (returns a string variant)  
            var pricingVariant = await _posthog.GetFeatureFlagAsync(
                userId, "pricing-experiment");

            // Capture the flag value alongside an event so it  
            // shows up correctly in PostHog insights  
            _posthog.Capture(userId, "feature_flag_checked", new Dictionary<string, object>
            {
                // Replace the following line in the FeatureFlags method:
                ["$feature/new-dashboard"] = newDashboardEnabled.ToString(),

                // With this corrected line:
                ["$feature/new-dashboard"] = newDashboardEnabled.ToString(),
                ["$feature/new-dashboard"] = newDashboardEnabled.ToString(),
                ["$feature/pricing-experiment"] = pricingVariant?.ToString()
            });

            ViewBag.NewDashboard = newDashboardEnabled;
            ViewBag.PricingVariant = pricingVariant;
            return View("FeatureFlags");
        }

        // ─────────────────────────────────────────────  
        // 3. EXPERIMENTS (A/B TESTING) – render variant UI  
        // ─────────────────────────────────────────────  
        public async Task<IActionResult> Experiment(string userId)
        {
            // Evaluate the experiment flag  
            var variant = await _posthog.GetFeatureFlagAsync(
                userId, "checkout-button-experiment");

            // Log an exposure event so PostHog can track  
            // who saw which variant  
            _posthog.Capture(userId, "$feature_flag_called", new Dictionary<string, object>
            {
                ["$feature_flag"] = "checkout-button-experiment",
                ["$feature_flag_value"] = variant?.ToString()
            });

            ViewBag.Variant = variant?.ToString() ?? "control";
            ViewBag.UserId = userId;
            return View("Experiment");
        }

        // Capture conversion event for experiment metric  
        [HttpPost]
        public IActionResult ExperimentConvert(string userId, string variant)
        {
            _posthog.Capture(userId, "checkout_button_clicked", new Dictionary<string, object>
            {
                ["$feature/checkout-button-experiment"] = variant
            });

            ViewBag.Message = "Conversion event sent!";
            return View("Result");
        }

        // ─────────────────────────────────────────────  
        // 4. GROUP ANALYTICS – track at org/company level  
        // ─────────────────────────────────────────────  
        public IActionResult GroupAnalytics(string userId, string companyId)
        {
            // Associate event with a group (e.g. a company)  
            _posthog.Capture(userId, "report_generated", new Dictionary<string, object>
            {
                ["$groups"] = new Dictionary<string, string>
                {
                    ["company"] = companyId
                },
                ["report_type"] = "monthly_summary"
            });

            ViewBag.Message = $"Event captured for user {userId} in company {companyId}";
            return View("Result");
        }

        // ─────────────────────────────────────────────  
        // 5. ERROR TRACKING – capture exceptions  
        // ─────────────────────────────────────────────  
        public IActionResult ErrorDemo(string userId)
        {
            try
            {
                // Simulate an error  
                throw new InvalidOperationException("Simulated payment failure");
            }
            catch (Exception ex)
            {
                // Capture the error as a structured event  
                _posthog.Capture(userId, "$exception", new Dictionary<string, object>
                {
                    ["$exception_type"] = ex.GetType().Name,
                    ["$exception_message"] = ex.Message,
                    ["$exception_stack"] = ex.StackTrace,
                    ["page"] = "/checkout"
                });

                ViewBag.Message = $"Error captured: {ex.Message}";
                return View("Result");
            }
        }

        // ─────────────────────────────────────────────  
        // 6. USER FUNNEL SIMULATION – for funnel insights  
        // ─────────────────────────────────────────────  
        public IActionResult SimulateFunnel(string userId)
        {
            // Simulate a realistic multi-step funnel  
            _posthog.Capture(userId, "viewed_pricing_page");
            _posthog.Capture(userId, "clicked_start_trial");
            _posthog.Capture(userId, "completed_onboarding", new Dictionary<string, object>
            {
                ["steps_completed"] = 4,
                ["time_to_complete_seconds"] = 142
            });
            _posthog.Capture(userId, "first_purchase", new Dictionary<string, object>
            {
                ["amount"] = 49.99,
                ["currency"] = "USD",
                ["plan"] = "pro"
            });

            ViewBag.Message = "Funnel events captured (pricing → trial → onboarding → purchase)";
            return View("Result");
        }
    }
}