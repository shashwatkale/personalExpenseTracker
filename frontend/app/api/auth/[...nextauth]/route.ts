import NextAuth, { NextAuthOptions } from 'next-auth';
import CredentialsProvider from 'next-auth/providers/credentials';
import GoogleProvider from 'next-auth/providers/google';
import { authApi } from '@/lib/api/services';

export const authOptions: NextAuthOptions = {
  providers: [
    GoogleProvider({
      clientId: process.env.GOOGLE_CLIENT_ID!,
      clientSecret: process.env.GOOGLE_CLIENT_SECRET!,
    }),
    CredentialsProvider({
      name: 'Credentials',
      credentials: {
        email: { label: 'Email', type: 'email' },
        password: { label: 'Password', type: 'password' },
      },
      async authorize(credentials) {
        if (!credentials?.email || !credentials?.password) {
          throw new Error('Invalid credentials');
        }

        try {
          const response = await authApi.login({
            email: credentials.email,
            password: credentials.password,
          });

          if (response.token) {
            return {
              id: response.email,
              email: response.email,
              name: `${response.firstName} ${response.lastName}`,
              accessToken: response.token,
            };
          }
          return null;
        } catch (error) {
          throw new Error('Invalid credentials');
        }
      },
    }),
  ],
  callbacks: {
    async jwt({ token, user, account }) {
      if (user) {
        token.accessToken = (user as any).accessToken;
        if (account?.provider === 'google') {
          // Register/login Google user with backend
          try {
            const response = await authApi.loginWithGoogle({
              email: user.email!,
              name: user.name!,
              googleId: user.id,
            });
            token.accessToken = response.token;
          } catch (error) {
            console.error('Google auth error:', error);
          }
        }
      }
      return token;
    },
    async session({ session, token }) {
      (session as any).accessToken = token.accessToken;
      return session;
    },
  },
  pages: {
    signIn: '/login',
  },
  session: {
    strategy: 'jwt',
  },
  secret: process.env.NEXTAUTH_SECRET,
};

const handler = NextAuth(authOptions);
export { handler as GET, handler as POST };
