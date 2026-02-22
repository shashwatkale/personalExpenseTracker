# Frontend - Next.js 14

## Tech Stack

- Next.js 14 (App Router)
- TypeScript
- Tailwind CSS
- NextAuth.js
- React Query
- Recharts

## Project Structure

```
frontend/
├── app/
│   ├── (auth)/
│   │   ├── login/          # Login page
│   │   └── register/       # Register page
│   ├── (dashboard)/
│   │   ├── dashboard/      # Main dashboard
│   │   └── expenses/       # Expense management
│   ├── api/
│   │   └── auth/           # NextAuth configuration
│   ├── layout.tsx          # Root layout
│   ├── providers.tsx       # React Query & NextAuth providers
│   └── globals.css         # Global styles
├── components/
│   ├── ui/                 # Reusable UI components
│   └── dashboard/          # Dashboard-specific components
├── hooks/
│   └── useExpenses.ts      # Custom React Query hooks
├── lib/
│   └── api/
│       ├── client.ts       # Axios client with interceptors
│       └── services.ts     # API service functions
└── package.json
```

## Setup

```bash
# Install dependencies
npm install

# Copy environment variables
cp .env.example .env.local

# Update .env.local with your values
# NEXT_PUBLIC_API_URL=http://localhost:5000/api
# NEXTAUTH_SECRET=generate-a-secret-key

# Run development server
npm run dev
```

## Features

### Authentication
- Login/Register with NextAuth
- JWT token management
- Protected routes

### Dashboard
- Monthly spending overview
- Category breakdown chart
- AI-powered insights
- Recent expenses list

### Expense Management
- Add/Edit/Delete expenses
- Category selection
- Date filtering
- Notes support

## API Integration

All API calls go through the centralized client in `lib/api/client.ts`:

```typescript
import apiClient from '@/lib/api/client';

// Automatically includes JWT token
const response = await apiClient.get('/expenses');
```

## Custom Hooks

```typescript
// Fetch expenses with React Query
const { data: expenses } = useExpenses();

// Create expense with mutation
const createExpense = useCreateExpense();
await createExpense.mutateAsync(expenseData);
```

## Environment Variables

- `NEXT_PUBLIC_API_URL`: .NET API base URL
- `NEXTAUTH_URL`: Frontend URL
- `NEXTAUTH_SECRET`: Secret for NextAuth (generate with `openssl rand -base64 32`)

## Build for Production

```bash
npm run build
npm start
```
