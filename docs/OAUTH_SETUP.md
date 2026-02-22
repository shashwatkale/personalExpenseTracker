# Google OAuth & Phone Authentication Setup

## Google OAuth Setup

### 1. Create Google Cloud Project

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing one
3. Enable Google+ API

### 2. Configure OAuth Consent Screen

1. Navigate to **APIs & Services** → **OAuth consent screen**
2. Choose **External** user type
3. Fill in required fields:
   - App name: `Expense Tracker`
   - User support email: your email
   - Developer contact: your email
4. Add scopes:
   - `userinfo.email`
   - `userinfo.profile`
5. Save and continue

### 3. Create OAuth Credentials

1. Go to **APIs & Services** → **Credentials**
2. Click **Create Credentials** → **OAuth client ID**
3. Choose **Web application**
4. Configure:
   - Name: `Expense Tracker Web Client`
   - Authorized JavaScript origins:
     - `http://localhost:3000`
     - `https://yourdomain.com` (production)
   - Authorized redirect URIs:
     - `http://localhost:3000/api/auth/callback/google`
     - `https://yourdomain.com/api/auth/callback/google` (production)
5. Click **Create**
6. Copy **Client ID** and **Client Secret**

### 4. Configure Frontend Environment

Update `frontend/.env.local`:

```env
GOOGLE_CLIENT_ID=your-client-id-here.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=your-client-secret-here
```

### 5. Test Google Login

1. Start all services
2. Navigate to `http://localhost:3000/login`
3. Click "Sign in with Google"
4. Authorize the app
5. You'll be redirected to dashboard

---

## Phone Authentication Setup

### Current Implementation (Development)

The phone authentication currently uses **in-memory storage** for verification codes and logs them to console. This is for **development only**.

### Production Setup Options

#### Option 1: Twilio (Recommended)

1. **Sign up for Twilio**
   - Go to [Twilio](https://www.twilio.com/)
   - Create account and get phone number

2. **Install Twilio SDK**
   ```bash
   cd backend/src/ExpenseTracker.Infrastructure
   dotnet add package Twilio
   ```

3. **Create Twilio Service**
   ```csharp
   // Infrastructure/Services/TwilioService.cs
   using Twilio;
   using Twilio.Rest.Api.V2010.Account;
   using Twilio.Types;
   
   public class TwilioService
   {
       private readonly string _accountSid;
       private readonly string _authToken;
       private readonly string _phoneNumber;
   
       public TwilioService(IConfiguration config)
       {
           _accountSid = config["Twilio:AccountSid"];
           _authToken = config["Twilio:AuthToken"];
           _phoneNumber = config["Twilio:PhoneNumber"];
           TwilioClient.Init(_accountSid, _authToken);
       }
   
       public async Task SendSmsAsync(string to, string message)
       {
           var messageResource = await MessageResource.CreateAsync(
               body: message,
               from: new PhoneNumber(_phoneNumber),
               to: new PhoneNumber(to)
           );
       }
   }
   ```

4. **Update AuthService**
   ```csharp
   public async Task<PhoneCodeResponse> SendPhoneVerificationAsync(SendPhoneCodeRequest request)
   {
       var code = new Random().Next(100000, 999999).ToString();
       _phoneCodes[request.PhoneNumber] = code;
       
       // Send via Twilio
       await _twilioService.SendSmsAsync(
           request.PhoneNumber, 
           $"Your Expense Tracker verification code is: {code}"
       );
       
       return new PhoneCodeResponse($"Verification code sent to {request.PhoneNumber}");
   }
   ```

5. **Add to appsettings.json**
   ```json
   {
     "Twilio": {
       "AccountSid": "your-account-sid",
       "AuthToken": "your-auth-token",
       "PhoneNumber": "+1234567890"
     }
   }
   ```

#### Option 2: AWS SNS

1. **Install AWS SDK**
   ```bash
   dotnet add package AWSSDK.SimpleNotificationService
   ```

2. **Create SNS Service**
   ```csharp
   using Amazon.SimpleNotificationService;
   using Amazon.SimpleNotificationService.Model;
   
   public class SnsService
   {
       private readonly IAmazonSimpleNotificationService _snsClient;
   
       public SnsService()
       {
           _snsClient = new AmazonSimpleNotificationServiceClient();
       }
   
       public async Task SendSmsAsync(string phoneNumber, string message)
       {
           var request = new PublishRequest
           {
               Message = message,
               PhoneNumber = phoneNumber
           };
           await _snsClient.PublishAsync(request);
       }
   }
   ```

#### Option 3: Firebase Authentication

1. Install Firebase Admin SDK
2. Use Firebase Phone Authentication
3. Verify tokens on backend

---

## Database Migration

After adding GoogleId and PhoneNumber fields to User entity, create and apply migration:

```bash
cd backend/src/ExpenseTracker.API

# Create migration
dotnet ef migrations add AddOAuthFields \
  --project ../ExpenseTracker.Infrastructure \
  --startup-project .

# Apply migration
dotnet ef database update
```

---

## Security Considerations

### Google OAuth
- ✅ Never commit client secrets to git
- ✅ Use environment variables
- ✅ Validate tokens on backend
- ✅ Use HTTPS in production
- ✅ Implement CSRF protection

### Phone Authentication
- ✅ Rate limit SMS sending (prevent abuse)
- ✅ Expire verification codes (5-10 minutes)
- ✅ Limit verification attempts (3-5 tries)
- ✅ Use secure random code generation
- ✅ Store codes securely (Redis recommended)
- ✅ Validate phone number format

---

## Testing

### Test Google Login
```bash
# 1. Start services
docker-compose up -d

# 2. Navigate to login page
open http://localhost:3000/login

# 3. Click "Sign in with Google"
# 4. Authorize app
# 5. Verify redirect to dashboard
```

### Test Phone Login (Development)
```bash
# 1. Start services
# 2. Click "Sign in with Phone"
# 3. Enter phone number: +1234567890
# 4. Click "Send Verification Code"
# 5. Check backend console for code
# 6. Enter code and verify
```

---

## Production Checklist

### Google OAuth
- [ ] Configure production redirect URIs
- [ ] Add production domain to authorized origins
- [ ] Store secrets in secure vault (Azure Key Vault, AWS Secrets Manager)
- [ ] Enable HTTPS
- [ ] Implement proper error handling
- [ ] Add logging for auth events

### Phone Authentication
- [ ] Integrate with Twilio/AWS SNS
- [ ] Move verification codes to Redis
- [ ] Implement rate limiting
- [ ] Add code expiration (5-10 minutes)
- [ ] Limit verification attempts
- [ ] Add phone number validation
- [ ] Implement retry logic
- [ ] Add monitoring and alerts

---

## API Endpoints

### Google Login
```http
POST /api/auth/google
Content-Type: application/json

{
  "email": "user@gmail.com",
  "name": "John Doe",
  "googleId": "google-user-id"
}
```

### Send Phone Verification
```http
POST /api/auth/phone/send-code
Content-Type: application/json

{
  "phoneNumber": "+1234567890"
}
```

### Verify Phone & Login
```http
POST /api/auth/phone
Content-Type: application/json

{
  "phoneNumber": "+1234567890",
  "verificationCode": "123456"
}
```

---

## Troubleshooting

### Google OAuth Issues

**Error: redirect_uri_mismatch**
- Ensure redirect URI in Google Console matches exactly
- Include protocol (http/https)
- Check for trailing slashes

**Error: invalid_client**
- Verify Client ID and Secret are correct
- Check environment variables are loaded

### Phone Authentication Issues

**Code not received**
- Check backend console logs (development)
- Verify Twilio/SNS credentials (production)
- Check phone number format (+country code)

**Invalid verification code**
- Code may have expired
- Check for typos
- Verify code storage is working

---

## Cost Considerations

### Twilio Pricing
- SMS: ~$0.0075 per message (US)
- Phone number: ~$1/month
- Free trial credits available

### AWS SNS Pricing
- SMS: $0.00645 per message (US)
- No monthly fees
- Free tier: 100 SMS/month

### Firebase
- Free tier: 10K verifications/month
- Pay as you go after that

---

## Next Steps

1. ✅ Setup Google OAuth credentials
2. ✅ Test Google login flow
3. ✅ Choose SMS provider (Twilio/AWS SNS)
4. ✅ Integrate SMS service
5. ✅ Test phone authentication
6. ✅ Implement rate limiting
7. ✅ Add monitoring
8. ✅ Deploy to production
