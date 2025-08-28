/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}", // scan Angular templates + TS
  ],
  theme: {
    extend: {},
  },
  plugins: [require("daisyui")],
};
