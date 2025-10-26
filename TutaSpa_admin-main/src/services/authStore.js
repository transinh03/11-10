// src/stores/authStore.js
import { defineStore } from 'pinia'
import { authAPI } from './authservice'


const checkAuth = () => {
  const token = localStorage.getItem('accessToken')
  if(token) {
    return true
  }
  else{
    try {
      const result = authAPI.refreshToken() 
      state.token = result.accessToken
      return true
    }
    catch{
      return false
    }
  }
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('accessToken') || null,
    role: localStorage.getItem('role') || null
  }),
  actions: {
    login(token, role) {
      this.token = token
      this.role = role
      localStorage.setItem('accessToken', token)
      localStorage.setItem('role', role)
    },
    logout() {
      this.token = null
      this.role = null
      localStorage.removeItem('accessToken')
      localStorage.removeItem('role')
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
