/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'noizera.com',
      },
      {
        protocol: 'http',
        hostname: '161.35.83.203',
      },
      {
        protocol: 'http',
        hostname: 'localhost',
      },
    ],
  },
  staticPageGenerationTimeout: 120,
};

module.exports = nextConfig;
