// src/chatService.js
import * as signalR from '@microsoft/signalr'
import router from '../router/router'
import { authAPI } from './authservice'

const chatHubUrl = import.meta.env.VITE_CHAT_URL
let connection = null
let pendingEventHandlers = []
let isConnecting = false

export async function createSignalRConnection(token) {
  // Nếu đang connecting thì đợi
  if (isConnecting) {
    while (isConnecting) {
      await new Promise(resolve => setTimeout(resolve, 100))
    }
    return connection
  }

  // Nếu đã có connection và đang connected
  if (connection && connection.state === signalR.HubConnectionState.Connected) {
    return connection
  }

  // Nếu có connection cũ nhưng không connected, dọn dẹp trước
  if (connection) {
    try {
      await connection.stop()
    } catch (error) {
      console.warn('Error stopping old connection:', error)
    }
    connection = null
  }

  isConnecting = true

  try {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(chatHubUrl, {
        accessTokenFactory: () => {
          // Lấy token mới nhất từ localStorage
          return localStorage.getItem('accessToken') || token
        }
      })
      .withAutomaticReconnect([0, 2000, 10000, 30000]) // Retry intervals
      .configureLogging(signalR.LogLevel.Information)
      .build()

    // Event handlers cho connection lifecycle
    connection.onreconnecting((error) => {
      console.warn('SignalR connection lost, attempting to reconnect...', error)
    })

    connection.onreconnected((connectionId) => {
      console.log('SignalR reconnected successfully. Connection ID:', connectionId)
      // Re-join group sau khi reconnect
      rejoinGroup()
    })

    connection.onclose((error) => {
      console.error('SignalR connection closed:', error)
      connection = null
    })

    // Bắt đầu connection
    await connection.start()
    console.log('SignalR connection established. Connection ID:', connection.connectionId)
    await connection.invoke('JoinAsAdmin')
    registerPendingEvents()
  } catch (error) {
    // Nếu lỗi unauthorized thì thử refresh token qua authAPI
    if (error.message && error.message.includes('unauthorized')) {
      try {
        // Gọi hàm refreshToken từ authAPI
        const response = await authAPI.refreshToken()
        const newToken = response.accessToken
        if (newToken) {
          localStorage.setItem('accessToken', newToken)
          // Thử kết nối lại với token mới
          return await createSignalRConnection(newToken)
        } else {
          router.push('/login?return_url=chat')
        }
      } catch (refreshError) {
        console.error('Refresh token failed:', refreshError)
        router.push('/login?return_url=chat')
      }
    } else {
      console.error('Error establishing SignalR connection:', error)
    }
    connection = null
    throw error
  } finally {
    isConnecting = false
  }
  return connection
}

// Function để rejoin group sau reconnection
async function rejoinGroup() {
  if (connection && connection.state === signalR.HubConnectionState.Connected) {
    try {
      await connection.invoke('JoinAsAdmin')
      console.log('Rejoined admin group after reconnection')
    } catch (error) {
      if(error.message.includes('unauthorized')) {
        router.push('/login?return_url=chat')
}}}}

// Register tất cả pending events
function registerPendingEvents() {
  if (connection && connection.state === signalR.HubConnectionState.Connected) {
    pendingEventHandlers.forEach(({ event, handler }) => {
      connection.on(event, handler)
    })
    console.log(`Registered ${pendingEventHandlers.length} pending event handlers`)
    pendingEventHandlers = []
  }
}

export function getSignalRConnection() {
  return connection
}

// Hàm helper để register events an toàn
export function registerSignalREvent(eventName, handler) {
  if (connection && connection.state === signalR.HubConnectionState.Connected) {
    connection.on(eventName, handler)
    console.log(`Registered event: ${eventName}`)
  } else {
    // Store event handler để register sau khi connection ready
    pendingEventHandlers.push({ event: eventName, handler })
    console.log(`Queued event: ${eventName}`)
  }
}

// Hàm để manually reconnect
export async function reconnectSignalR() {
  const token = localStorage.getItem('accessToken')
  if (!token) {
    console.error('No access token found for reconnection')
    return null
  }

  try {
    return await createSignalRConnection(token)
  } catch (error) {
    console.error('Manual reconnection failed:', error)
    return null
  }
}

// Check connection status
export function getConnectionState() {
  if (!connection) return 'Disconnected'
  
  switch (connection.state) {
    case signalR.HubConnectionState.Connected:
      return 'Connected'
    case signalR.HubConnectionState.Connecting:
      return 'Connecting'
    case signalR.HubConnectionState.Reconnecting:
      return 'Reconnecting'
    case signalR.HubConnectionState.Disconnected:
      return 'Disconnected'
    case signalR.HubConnectionState.Disconnecting:
      return 'Disconnecting'
    default:
      return 'Unknown'
  }
}

// Check if connection is ready for invoke
export function isConnectionReady() {
  return connection && connection.state === signalR.HubConnectionState.Connected
}

export function stopSignalRConnection() {
  if (connection) {
    connection.stop()
    connection = null
  }
  pendingEventHandlers = []
  isConnecting = false
}

// Auto-reconnect function để gọi từ component
export async function ensureConnection() {
  if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
    const token = localStorage.getItem('accessToken')
    if (token) {
      return await createSignalRConnection(token)
    }
  }
  return connection
}