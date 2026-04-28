# Copilot Instructions

## Project Guidelines
- Use the existing project-specific folder structure; do not place all controllers in a single shared controllers folder.
- Use .NET 8 (LTS) for project targets and package/tool recommendations; avoid .NET 10 suggestions unless explicitly requested.
- Do not hardcode runtime values; use configuration/environment variables because services are developed independently and merged later.
- Prioritize implementing only Sahil-owned modules: authentication (service + MVC), profile, notifications (service + MVC), and calculator (service + MVC). Avoid expanding into claim/policy/support implementations beyond what is necessary for those flows while keeping the existing institutional UI style, with .NET 8/C#12 conventions for service and frontend work.
- Use simple, user-friendly authentication UI text (e.g., Password, Remember me) and prefer black styling over green accent on the login card theme.