✅ 1. Register an App in Entra ID

Go to:
Microsoft Entra admin center

Steps:

Navigate to Identity → Applications → App registrations
Click New registration
Name your app (e.g., Taktiks-Graph-Client)
Choose supported account type (usually Single tenant)
Register
✅ 2. Configure Authentication

Inside your app:

Go to:

Authentication → Add platform

Choose based on your app type:

Web app → Web
SPA (Angular/React) → Single-page application

Add redirect URI, e.g.:

https://localhost:4200
https://yourdomain.com/auth/callback

Enable:

✔ Access tokens
✔ ID tokens
✅ 3. Add Microsoft Graph API Permissions

Go to:
API permissions → Add permission → Microsoft Graph

Add Delegated permissions (recommended for user login flow):
Calendars.ReadWrite
OnlineMeetings.ReadWrite
⚠️ 4. Grant Admin Consent (required in your case)

Since OnlineMeetings.ReadWrite often requires admin consent:

Click:

Grant admin consent for [Tenant Name]

Confirm.

✔ This step enables your app to call Graph APIs without each user approving permissions.

✅ 5. Create Client Secret (if backend is used)

Go to:
Certificates & secrets

Click New client secret
Copy value immediately (you won’t see it again)
✅ 6. Implement Authentication (MSAL)
If Angular / SPA:

Use MSAL:

Login user
Acquire token with scopes:
Calendars.ReadWrite
OnlineMeetings.ReadWrite
If Backend (.NET):

Use:

MSAL.NET
Or Microsoft.Identity.Web
✅ 7. Acquire Access Token

Scopes example:

https://graph.microsoft.com/Calendars.ReadWrite
https://graph.microsoft.com/OnlineMeetings.ReadWrite
✅ 8. Call Microsoft Graph APIs
📅 Create Calendar Event
POST https://graph.microsoft.com/v1.0/me/events
🎥 Create Online Meeting
POST https://graph.microsoft.com/v1.0/me/onlineMeetings
✅ 9. Example Headers
Authorization: Bearer ACCESS_TOKEN
Content-Type: application/json
✅ 10. Verify Permissions Work

Use Graph Explorer:
Microsoft Graph Explorer

Test:

/me/events
/me/onlineMeetings
⚠️ Common Issues
❌ “Insufficient privileges”
Admin consent not granted
❌ “Need application permission”
You are using app-only token instead of delegated flow
❌ Calendar works but meetings fail
OnlineMeetings.ReadWrite missing admin consent
🧠 Simple Architecture Summary
SPA / Angular:

User logs in → MSAL → Delegated token → Call Graph API

Backend (.NET):

User login → token acquisition → GraphServiceClient → API calls

If you want, I can next give you:

Full ASP.NET Core working sample
Angular MSAL implementation
Or 
Postman setup to test everything quickly

Just tell me 👍
