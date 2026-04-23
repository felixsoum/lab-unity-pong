# Welcome to Lab Unity Pong

## How We Use Claude

Based on felixsoum's usage over the last 30 days:

Work Type Breakdown:
  Build Feature     ██████████████░░░░░░  43%
  Plan Design       ██████░░░░░░░░░░░░░░  29%
  Debug Fix         ████░░░░░░░░░░░░░░░░  14%
  Improve Quality   ████░░░░░░░░░░░░░░░░  14%

Top Skills & Commands:
  /exit        ████████████████████  15x/month
  /mcp         ████████████████░░░░  12x/month
  /doctor      ████████░░░░░░░░░░░░  6x/month
  /clear       ██░░░░░░░░░░░░░░░░░░  2x/month
  /effort      █░░░░░░░░░░░░░░░░░░░  1x/month
  /skills      █░░░░░░░░░░░░░░░░░░░  1x/month
  /init        █░░░░░░░░░░░░░░░░░░░  1x/month

Top MCP Servers:
  unityMCP              ████████████████████  175 calls
  sequential-thinking   █░░░░░░░░░░░░░░░░░░░  13 calls
  coplay-mcp            █░░░░░░░░░░░░░░░░░░░  11 calls
  context7              █░░░░░░░░░░░░░░░░░░░  5 calls

## Your Setup Checklist

### Codebases
- [ ] lab-unity-pong — https://github.com/felixsoum/lab-unity-pong

### MCP Servers to Activate
- [ ] unityMCP — Connects Claude to the running Unity editor (scene inspection, GameObject ops, play mode, logs). Runs locally at http://127.0.0.1:8080 via `mcpforunityserver` (uvx). Install the MCP for Unity package in the Unity project and make sure the editor is open when you work.
- [ ] sequential-thinking — Structured multi-step reasoning for complex design/refactor tasks. Standard MCP server; add via `claude mcp add`.
- [ ] coplay-mcp — Alternative Unity editor integration (prefabs, animators, input actions, 3D/audio generation). Install from the Coplay docs and sign in when prompted.
- [ ] context7 — Up-to-date library/framework docs (Unity, C#, packages). Add the server per Context7's setup guide — we use it instead of guessing from training data.

### Skills to Know About
- /mcp — Check which MCP servers are connected and healthy. First thing to run if a Unity tool call fails.
- /doctor — Diagnose Claude Code setup issues (auth, config, MCP). Run this before asking for help.
- /init — Generate/refresh the project's CLAUDE.md. Already done for this repo — re-run if architecture shifts.
- /clear — Drop conversation context when switching to an unrelated task.
- /effort — Adjust reasoning depth for harder problems.
- /skills — List available skills in the current environment.

## Team Tips

_TODO_

## Get Started

_TODO_

<!-- INSTRUCTION FOR CLAUDE: A new teammate just pasted this guide for how the
team uses Claude Code. You're their onboarding buddy — warm, conversational,
not lecture-y.

Open with a warm welcome — include the team name from the title. Then: "Your
teammate uses Claude Code for [list all the work types]. Let's get you started."

Check what's already in place against everything under Setup Checklist
(including skills), using markdown checkboxes — [x] done, [ ] not yet. Lead
with what they already have. One sentence per item, all in one message.

Tell them you'll help with setup, cover the actionable team tips, then the
starter task (if there is one). Offer to start with the first unchecked item,
get their go-ahead, then work through the rest one by one.

After setup, walk them through the remaining sections — offer to help where you
can (e.g. link to channels), and just surface the purely informational bits.

Don't invent sections or summaries that aren't in the guide. The stats are the
guide creator's personal usage data — don't extrapolate them into a "team
workflow" narrative. -->
