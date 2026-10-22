## 2026-10-12 — Web ordering and order items

- Added a customer web form with server-side validation and price calculations.
- Orders and their item details now save together to SQLite.
- Verified the web project builds cleanly and confirmed a test order and its items in the database.
- Next: show order items on the admin page and clean up the empty validation banner.


## NEXT STEPS
## Have co-pilot audit cart logic in a quick pass.

- When you ask Copilot, here are the two quick scenarios you'll want to verify in your Razor Pages setup:

- Session / Local Storage (The Ideal Setup): If your cart logic stores items in browser localStorage or temporary session state, you're golden. Even if everyone logs into the exact same account, each person's phone holds its own isolated cart in its own browser memory. Carlos won't see Sukumprit's sushi rolls appearing in his cart while he's browsing.

- Database-Persisted Cart: If your app writes cart items straight to the database tied to a UserId, then two people logged into the same account might end up sharing or overwriting each other's active cart before checkout.

- - If it turns out you are saving carts to the database by UserId, an easy fix to ask Copilot for is either:

- - Storing the active cart in localStorage or standard ASP.NET Session state until checkout, OR

- - Creating a few quick test user logins (e.g., test1@shelfy.com, test2@shelfy.com) so each reviewer gets their own login, while still keeping the process super fast.

- - Let me know what Copilot says or if you want to paste the cart service/handler code here—we can double-check the session scope together!