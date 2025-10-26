// src/stores/authStore.js
import { defineStore } from 'pinia'


export const useAuthState = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('accessToken') || null,
    user_info: JSON.parse(localStorage.getItem('user_info')) || null,
  }),
  actions: {
    login(token, user_info) {
      this.token = token
      this.user_info = user_info
      localStorage.setItem('accessToken', token)
        localStorage.setItem('user_info', JSON.stringify(user_info))
    },
    logout() {
      this.token = null
      this.user_info = null
      localStorage.removeItem('accessToken')
      localStorage.removeItem('user_info')
    },
    getToken () {
      return this.token
    },
  },
  getters: {
    isAuthenticated: state => !!state.token,
    isCashier: state => state.role === 'Cashier',
    isInventoryManager: state => state.role === 'InventoryManager',
    isAdmin: state => state.role === 'Admin',
    isManager: state => state.role === 'manager',
  }
})
