/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'spa-jewel': '#1e3a8a', // Deep sapphire
        'spa-emerald': '#059669', // Vibrant emerald
        'spa-gold': '#f1c40f', // Bright gold
        'spa-rose': '#f43f5e', // Rose accent
        'spa-dark': '#1f2937', // Deep charcoal
        'spa-bg-start': '#dbeafe', // Soft blue
        'spa-bg-end': '#fce7f3', // Soft pink
      },
      fontFamily: {
        playfair: ['Playfair Display', 'serif'],
        lora: ['Lora', 'serif'],
      },
      animation: {
        'wave': 'wave 10s ease-in-out infinite',
        'pulse-glow': 'pulseGlow 2s ease-in-out infinite',
      },
      keyframes: {
        wave: {
          '0%, 100%': { transform: 'translateY(0)' },
          '50%': { transform: 'translateY(-10px)' },
        },
        pulseGlow: {
          '0%, 100%': { boxShadow: '0 0 10px rgba(241, 196, 15, 0.5)' },
          '50%': { boxShadow: '0 0 20px rgba(241, 196, 15, 0.8)' },
        },
      },
    },
  },
  plugins: [],
}