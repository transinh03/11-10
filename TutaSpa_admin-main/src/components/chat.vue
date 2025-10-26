<template>
  <div class="chat-container">
    <!-- Sidebar -->
    <div class="sidebar">
      <div class="sidebar-header">
        <h2 class="section-title">
          <i class="fas fa-comments"></i>
          Đoạn chat
        </h2>
        <div class="header-actions">
          <button class="btn btn-outline-primary" @click="refreshSessions">
            <i class="fas fa-sync-alt"></i>
            Làm mới
          </button>
        </div>
      </div>

      <!-- Search -->
      <div class="search-container">
        <div class="search-box">
          <i class="fas fa-search search-icon" @click="handleSearch"></i>
          <input type="text" placeholder="Tìm tên khách hàng" v-model="searchQuery" class="search-input" />
          <button v-if="searchQuery" @click="clearSearch" class="clear-search" title="Xóa tìm kiếm">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>

      <!-- Filters -->
      <div class="filters">
        <button 
          class="btn btn-outline-primary filter-btn "  
          :class="{ active: currentFilter === 'all' }"
          @click="setFilter('all')"
        >
          Tất cả
        </button>
        <button 
          class="btn btn-outline-primary filter-btn" 
          :class="{ active: currentFilter === 'online' }"
          @click="setFilter('online')"
        >
          Đang online
        </button>
        <button 
          class="btn btn-outline-primary filter-btn " 
          :class="{ active: currentFilter === 'unread' }"
          @click="setFilter('unread')"
        >
          Chưa đọc
        </button>
        <button 
          class="btn btn-outline-primary filter-btn " 
          :class="{ active: currentFilter === 'closed' }"
          @click="setFilter('closed')"
        >
          Đã đóng
        </button>
      </div>

      <!-- Loading indicator -->
      <div v-if="isLoading" class="loading-state">
        <i class="fas fa-spinner fa-spin"></i>
        Đang tải...
      </div>

      <!-- Customer List -->
      <div v-else class="customer-list">
        <div 
          v-for="customer in filteredCustomers" 
          :key="customer.sessionId || customer.userId"
          class="customer-item"
          :class="{ 
            active: (selectedCustomer && (
              (selectedCustomer.sessionId && selectedCustomer.sessionId === customer.sessionId) ||
              (selectedCustomer.userId && selectedCustomer.userId === customer.userId)
            )),
            online: customer.isOnline,
            'has-unread': customer.unreadCount > 0
          }"
          @click="selectCustomer(customer)"
        >
          <div class="customer-info">
            <div class="customer-name-row">
              <span class="customer-name">{{ customer.userName || customer.customerName }}</span>
            </div>
            <div class="last-message">
              <i v-if="customer.isFromAdmin" class="fas fa-reply message-icon"></i>
              {{ customer.lastMessage || 'Chưa có tin nhắn' }}
            </div>
            <div class="session-info">
              <span v-if="customer.messageCount" class="message-count">
                • {{ customer.messageCount }} tin nhắn
              </span>
            </div>
          </div>
          
          <div class="customer-meta">
            <div class="time">{{ formatTime(customer.lastMessageTime || customer.startTime, true, false) }}</div>
            <div v-if="customer.unreadCount > 0" class="unread-count">
              {{ customer.unreadCount }}
            </div>
            <div v-if="!customer.isOnline && customer.status !== 'Closed'" class="offline-indicator">
              <i class="fas fa-circle"></i>
            </div>
          </div>
        </div>

        <!-- Empty state for filter -->
        <div v-if="filteredCustomers.length === 0" class="empty-state">
          <i class="fas fa-search"></i>
          <p>Không tìm thấy cuộc trò chuyện nào</p>
        </div>
      </div>
    </div>

    <!-- Chat Area -->
    <div class="chat-area">
      <div v-if="selectedCustomer" class="chat-content">
        <!-- Chat Header -->
        <div class="chat-header">
          <div class="customer-info">
            <img 
              v-if="selectedCustomer.isOnline" 
              :src="'https://upload.wikimedia.org/wikipedia/commons/7/7c/Profile_avatar_placeholder_large.png?20150327203541'" 
              alt="Avatar" 
              class="customer-avatar">
            <div>
              <h3>{{ selectedCustomer.userName || selectedCustomer.customerName }}</h3>
              <div class="status-info">
                <span class="status" :class="{ online: selectedCustomer.isOnline }">
                  {{ selectedCustomer.isOnline ? 'Đang hoạt động' : 'Không hoạt động' }}
                </span>
              </div>
            </div>
          </div>
          <div class="chat-actions">
            <button 
              class="btn btn-danger btn-sm close-session-btn" 
              @click="closeSession" 
              v-if="selectedCustomer && selectedCustomer.status !== 'Closed'"
              title="Đóng phiên chat"
            >
              <i class="fas fa-lock"></i>
            </button>
          </div>
        </div>

        <!-- Messages -->
        <div class="messages-container" ref="messagesContainer" id="messagesContainer">
          <div v-if="isLoadingHistory" class="loading-state">
            <i class="fas fa-spinner fa-spin"></i>
            Đang tải lịch sử...
          </div>
          
          <div v-for="message in currentMessages" :key="message.messageId" class="message-wrapper">
            <div 
              class="message"
              :class="{ 'message-sent': message.isFromAdmin, 'message-received': !message.isFromAdmin }"
            >
              <div class="message-content">
                <p>{{ message.message }}</p>
              </div>
              <div class="message-time">
                {{ formatTime(message.timestamp) }}
                <i v-if="message.isRead && message.isFromAdmin" class="fas fa-check-double read-status"></i>
                <i v-else-if="message.isFromAdmin" class="fas fa-check sent-status"></i>
              </div>
            </div>
          </div>
          
          <!-- Load more button -->
          <div v-if="hasMoreMessages" class="load-more-container">
            <button class="btn btn-outline-primary" @click="loadMoreMessages" :disabled="isLoadingMore">
              <i v-if="isLoadingMore" class="fas fa-spinner fa-spin"></i>
              <span>{{ isLoadingMore ? 'Đang tải...' : 'Tải thêm tin nhắn' }}</span>
            </button>
          </div>
        </div>

        <!-- Closed notice -->
        <div v-if="selectedCustomer.status === 'Closed'" class="closed-notice">
          <i class="fas fa-lock"></i>
          Phiên chat đã kết thúc.
        </div>

        <!-- Input Area -->
        <div class="input-area" v-else>
          <div class="input-container">
            <div class="message-input">
              <input 
                type="text" 
                v-model="newMessage" 
                @keyup.enter="sendMessage"
                placeholder="Aa"
                ref="messageInput"
                :disabled="selectedCustomer.status === 'Closed'"
                class="form-control"
              />
            </div>
            <button 
              class="btn btn-success btn-sm send-btn" 
              @click="sendMessage" 
              :disabled="!newMessage.trim() || selectedCustomer.status === 'Closed'"
            >
              <i class="fas fa-paper-plane"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="empty-state">
        <div class="empty-icon">
          <i class="fas fa-comments"></i>
        </div>
        <h3>Chọn một cuộc trò chuyện</h3>
        <p>Chọn một khách hàng để bắt đầu trò chuyện</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onBeforeUnmount, nextTick, watch } from 'vue'
import * as signalR from '@microsoft/signalr'
import { useRouter } from 'vue-router'
import { getSignalRConnection, registerSignalREvent, getConnectionState, createSignalRConnection } from '../services/chatService'
import { authAPI } from '../services/authservice'
import apiClient from '../utils/axiosClient'

// Router instance
const router = useRouter()

// Environment variable
const base_url = import.meta.env.VITE_CHAT_URL

// State
const searchQuery = ref('')
const selectedCustomer = ref(null)
const newMessage = ref('')
const customers = ref([])
const recentSessions = ref([])
const messages = reactive({})
const currentFilter = ref('all')
const isLoading = ref(true)
const isLoadingHistory = ref(false)
const isLoadingMore = ref(false)
const hasMoreMessages = ref(false)
const currentPage = ref(1)
const sessionId = router.currentRoute.value.query.sessionId || null
const connectionReady = ref(false)
const adminInfo = {
  adminId: 'admin_001',
  adminName: 'Admin Support'
}

// Computed
const filteredCustomers = computed(() => {
  let filtered = [...customers.value]
  
  // Apply search filter  
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(customer =>
      (customer.userName || customer.customerName || '').toLowerCase().includes(query) ||
      (customer.lastMessage || '').toLowerCase().includes(query)
    )
  }
  
  // Apply category filter
  switch (currentFilter.value) {
    case 'online':
      filtered = filtered.filter(c => c.status !== 'Closed' && c.isOnline)
      break
    case 'unread':
      filtered = filtered.filter(c => c.unreadCount > 0)
      break
    case 'closed':
      filtered = filtered.filter(c => c.status === 'Closed')
      break
  }
  
  // Sort: online first, then by last message time
  return filtered.sort((a, b) => {
    if (a.isOnline && !b.isOnline) return -1
    if (!a.isOnline && b.isOnline) return 1
    
    const aTime = new Date(a.lastMessageTime || a.startTime)
    const bTime = new Date(b.lastMessageTime || b.startTime)
    return bTime - aTime
  })
})

const currentMessages = computed(() => {
  if (!selectedCustomer.value) return []
  const sessionKey = selectedCustomer.value.sessionId || selectedCustomer.value.userId
  return messages[sessionKey] || []
})

// Helper function to wait for connection
async function waitForConnection(maxAttempts = 30) {
  let attempts = 0
  
  while (attempts < maxAttempts) {
    const conn = getSignalRConnection()
    const state = getConnectionState()
    
    console.log(`Connection attempt ${attempts + 1}, state: ${state}`)
    
    if (conn && state === 'Connected') {
      connectionReady.value = true
      return conn
    }
    
    // Wait 500ms before next attempt
    await new Promise(resolve => setTimeout(resolve, 500))
    attempts++
  }
  
  throw new Error(`Connection not ready after ${maxAttempts} attempts`)
}

// Safe invoke function
async function safeInvoke(methodName, ...args) {
  try {
    const conn = await waitForConnection()
    return await conn.invoke(methodName, ...args)
  } catch (error) {
    if (error.message && error.message.includes('unauthorized')) {
      // Thử refresh token qua authAPI
      try {
        const response = await authAPI.refreshToken()
        const newToken = response.accessToken
        if (newToken) {
          localStorage.setItem('accessToken', newToken)
          // Tạo lại connection với token mới
          await createSignalRConnection(newToken)
          // Thử lại lệnh invoke
          const conn = await waitForConnection()
          return await conn.invoke(methodName, ...args)
        } else {
          router.push('/login?return_url=chat')
        }
      } catch (refreshError) {
        console.error('Refresh token failed:', refreshError)
        router.push('/login?return_url=chat')
      }
    }
    console.error(`Error invoking ${methodName}:`, error)
    throw error
  }
}

// Lifecycle hooks
onMounted(async () => {
  await initializeSignalR()

  // Lấy sessionId từ route và tự động chọn customer, load lịch sử
  const sessionIdFromRoute = router.currentRoute.value.query.sessionId
  console.log('Session ID from route:', sessionIdFromRoute)
  if (sessionIdFromRoute) {
    // Đợi dữ liệu load xong
    await nextTick()
    // Tìm customer theo sessionId
    const customer = customers.value.find(
      c => c.sessionId === sessionIdFromRoute
    )
    if (customer) {
      await selectCustomer(customer)
    } else {
      // Nếu chưa có trong danh sách, thử lấy lịch sử trực tiếp
      selectedCustomer.value = {
        sessionId: sessionIdFromRoute,
        userId: null,
        userName: 'Khách hàng',
        customerName: 'Khách hàng',
        isOnline: false,
        status: '',
        unreadCount: 0,
        lastMessage: '',
        lastMessageTime: '',
        isAssigned: false
      }
      await selectCustomer(selectedCustomer.value)
    }
  }
})

onBeforeUnmount(() => {
  // const conn = getSignalRConnection()
  // if (conn) {
  //   conn.stop()
  // }
})

// Watch for filter changes
watch(currentFilter, () => {
  // Reset selection if current customer is filtered out
  if (selectedCustomer.value && !filteredCustomers.value.find(c => 
    (c.sessionId || c.userId) === (selectedCustomer.value.sessionId || selectedCustomer.value.userId)
  )) {
    selectedCustomer.value = null
  }
})

// Functions
async function initializeSignalR() {
  try {
    const token = getAuthToken()
    
    // Create connection
    const conn = await createSignalRConnection(token)
    
    if (!conn) {
      throw new Error('Failed to create SignalR connection')
    }

    // Wait for connection to be ready
    await waitForConnection()

    // Đăng ký các event qua chatService để an toàn
    registerSignalREvent('NewCustomerJoined', handleNewCustomerJoined)
    registerSignalREvent('UserAssigned', handleUserAssigned)
    registerSignalREvent('ReceiveMessage', handleReceiveMessage)
    registerSignalREvent('CustomerDisconnected', handleChatClosed)
    registerSignalREvent('ChatHistory', handleChatHistory)
    registerSignalREvent('MessageSent', handleMessageSent)
    registerSignalREvent('AdminAssigned', handleAdminAssigned)
    registerSignalREvent('Error', handleError)
    registerSignalREvent('ChatClosed', handleChatClosed)

    // Lắng nghe sự kiện reconnect thành công để load lại dữ liệu
    conn.onreconnected(async () => {
      console.log('SignalR reconnected, reload data...')
      connectionReady.value = false
      await waitForConnection()
      await loadInitialData()
    })

    // Load initial data after connection is ready
    await loadInitialData()
  } catch (error) {
    console.error('SignalR Connection Error:', error)
    isLoading.value = false
    connectionReady.value = false
    
    // Retry after 3 seconds
    setTimeout(() => {
      console.log('Retrying SignalR connection...')
      initializeSignalR()
    }, 3000)
  }
}

async function loadInitialData() {
  try {
    // Ensure connection is ready
    await waitForConnection()
    
    console.log('Loading initial data...')

    const sessions = await apiClient.get("/chat/recent-sessions?hours=72&pagesize=50")
    console.log('Recent sessions loaded:', sessions)
    handleRecentSessions(sessions)
  } catch (error) {
    console.error('Error loading initial data:', error)
    // Retry after 2 seconds
    setTimeout(() => {
      console.log('Retrying load initial data...')
      loadInitialData()
    }, 2000)
  } finally {
    isLoading.value = false
  }
}

function handleChatClosed(data) {
  if (!data || !data.sessionId) return
  const session = recentSessions.value.find(s => s.sessionId === data.sessionId)
  if (session) {
    session.status = 'Closed'
  }
  const customer = customers.value.find(c => c.sessionId === data.sessionId)
  if (customer) {
    customer.status = 'Closed'
  }
  if (selectedCustomer.value && selectedCustomer.value.sessionId === data.sessionId) {
    selectedCustomer.value.status = 'Closed'
  }
}

function handleOnlineCustomers(onlineList) {
  console.log('Received online customers:', onlineList)
  onlineList.forEach(customer => {
    const existingIndex = customers.value.findIndex(c => 
      c.sessionId === customer.sessionId || c.userId === customer.userId
    )
    if (existingIndex !== -1) {
      Object.assign(customers.value[existingIndex], {
        ...customer,
        isOnline: true
      })
    } else {
      customers.value.unshift({
        ...customer,
        isOnline: true
      })
    }
  })
}

function handleRecentSessions(sessions) {
  console.log(sessions)
  recentSessions.value = sessions
  sessions.forEach(session => {
    const existingIndex = customers.value.findIndex(c => 
      c.sessionId === session.sessionId || c.userId === session.customerId
    )
    const customerData = {
      sessionId: session.sessionId,
      userId: session.customerId,
      userName: session.customerName,
      customerName: session.customerName,
      adminId: session.adminId,
      adminName: session.adminName,
      startTime: session.startTime,
      endTime: session.endTime,
      status: session.status,
      isOnline: session.isOnline || false,
      lastMessage: session.lastMessage,
      lastMessageTime: session.lastMessageTime,
      messageCount: session.messageCount,
      unreadCount: 0,
      isAssigned: !!session.adminId,
      isFromAdmin: session.lastMessage && session.lastMessage.includes('Xin chào')
    }
    if (existingIndex !== -1) {
      Object.assign(customers.value[existingIndex], customerData)
    } else {
      customers.value.push(customerData)
    }
  })
  customers.value.sort((a, b) => {
    if (a.isOnline && !b.isOnline) return -1
    if (!a.isOnline && b.isOnline) return 1
    const aTime = new Date(a.lastMessageTime || a.startTime)
    const bTime = new Date(b.lastMessageTime || b.startTime)
    return bTime - aTime
  })
}

function handleNewCustomerJoined(customer) {
  console.log('New customer joined:', customer)
  const existingIndex = customers.value.findIndex(c => 
    c.userId === customer.userId || c.sessionId === customer.sessionId
  )
  const customerData = {
    userId: customer.userId,
    sessionId: customer.sessionId,
    userName: customer.userName,
    customerName: customer.customerName || customer.userName,
    startTime: customer.startTime || customer.joinTime,
    isOnline: true,
    unreadCount: customer.unreadCount || 0,
    lastMessage: '',
    lastMessageTime: customer.startTime || customer.joinTime,
    isAssigned: customer.isAssigned || false,
    adminId: customer.assignedAdminId,
    adminName: customer.assignedAdminName
  }
  if (existingIndex !== -1) {
    Object.assign(customers.value[existingIndex], customerData)
  } else {
    customers.value.unshift(customerData)
  }
  if (customer.waitingMessages && customer.waitingMessages.length > 0) {
    customer.waitingMessages.forEach(message => {
      handleReceiveMessage(message)
    })
  }
}

function handleUserAssigned(assignment) {
  console.log('User assigned:', assignment)
  let customer = customers.value.find(c => c.userId === assignment.customerId)
  if (!customer) {
    customer = {
      sessionId: assignment.sessionId,
      userId: assignment.customerId,
      userName: assignment.customerName,
      customerName: assignment.customerName,
      startTime: assignment.startTime,
      isOnline: assignment.isOnline,
      lastMessage: assignment.lastMessage || '',
      lastMessageTime: assignment.lastMessageTime || assignment.startTime,
      unreadCount: assignment.unreadCount || 0,
      isAssigned: true,
      adminId: assignment.adminId || adminInfo.adminId,
      adminName: assignment.adminName || adminInfo.adminName,
      waitingMessages: assignment.waitingMessages || [],
      isFromAdmin: false
    }
    customers.value.unshift(customer)
  } else {
    customer.isAssigned = true
    customer.adminId = assignment.adminId || adminInfo.adminId
    customer.adminName = assignment.adminName || adminInfo.adminName
    customer.unreadCount = assignment.unreadCount || 0
  }
  if (assignment.waitingMessages && assignment.waitingMessages.length > 0) {
    assignment.waitingMessages.forEach(message => {
      handleReceiveMessage(message)
    })
  }
}

function handleReceiveMessage(message) {
  console.log('Received message:', message)
  const sessionKey = message.sessionId || message.fromUserId
  if (!messages[sessionKey]) {
    messages[sessionKey] = []
  }
  const existingMessage = messages[sessionKey].find(m => m.messageId === message.messageId)
  if (!existingMessage) {
    messages[sessionKey].push(message)
  }
  const customer = customers.value.find(c => 
    c.sessionId === sessionKey || 
    c.userId === message.fromUserId || 
    c.userId === message.toUserId
  )
  if (customer) {
    customer.lastMessage = message.message
    customer.lastMessageTime = message.timestamp
    customer.isFromAdmin = message.isFromAdmin
    if (!selectedCustomer.value || 
        (selectedCustomer.value.sessionId || selectedCustomer.value.userId) !== sessionKey) {
      if (!message.isFromAdmin) {
        customer.unreadCount = (customer.unreadCount || 0) + 1
      }
    }
    const index = customers.value.indexOf(customer)
    if (index > 0) {
      customers.value.splice(index, 1)
      customers.value.unshift(customer)
    }
  }
  scrollToBottom()
}

function handleCustomerDisconnected(customer) {
  console.log('Customer disconnected:', customer)
  const existingCustomer = customers.value.find(c => c.userId === customer.userId)
  if (existingCustomer) {
    existingCustomer.isOnline = false
  }
}

function handleChatHistory(history) {
  console.log('Received chat history:', history)
  isLoadingHistory.value = false
  const sessionKey = history.sessionId
  if (currentPage.value === 1) {
    messages[sessionKey] = history.messages || []
  } else {
    const existingMessages = messages[sessionKey] || []
    messages[sessionKey] = [...(history.messages || []), ...existingMessages]
  }
  hasMoreMessages.value = history.pageInfo?.hasMore || false
  isLoadingMore.value = false
  if (currentPage.value === 1) {
    nextTick(() => scrollToBottom())
  }
}

function handleMessageSent(message) {
  console.log('Message sent:', message)
}

function handleAdminAssigned(assignment) {
  console.log('Admin assigned:', assignment)
}

function handleError(error) {
  console.error('SignalR Error:', error)
}

async function selectCustomer(customer) {
  selectedCustomer.value = customer
  customer.unreadCount = 0
  currentPage.value = 1
  hasMoreMessages.value = false
  const sessionId = customer.sessionId || customer.userId
  router.replace({ path: '/chat', query: { sessionId } })
  if (!sessionId) {
    console.error('No sessionId or userId found for customer:', customer)
    return
  }
  const sessionKey = sessionId
  messages[sessionKey] = []
  isLoadingHistory.value = true
  try {
    const history = await apiClient.get(`/chat/history?sessionId=${sessionId}`)
    handleChatHistory(history)
  } catch (error) {
    console.error('Error selecting customer:', error)
    isLoadingHistory.value = false
  }
  nextTick(() => {
    const input = document.getElementById('messageInput')
    if (input) input.focus()
  })
}

async function loadChatHistory(customer) {
  try {
    await safeInvoke('GetChatHistory', customer.sessionId, 50, currentPage.value)
  } catch (error) {
    console.error('Error loading chat history:', error)
    isLoadingHistory.value = false
  }
}

async function loadMoreMessages() {
  if (!selectedCustomer.value || isLoadingMore.value) return
  isLoadingMore.value = true
  currentPage.value += 1
  try {
    await loadChatHistory(selectedCustomer.value)
  } catch (error) {
    console.error('Error loading more messages:', error)
    isLoadingMore.value = false
    currentPage.value -= 1
  }
}

async function closeSession() {
  if (!selectedCustomer.value || !selectedCustomer.value.sessionId) return
  try {
    console.log('Closing session:', selectedCustomer.value.sessionId)
    await safeInvoke('CloseSession', selectedCustomer.value.sessionId)
  } catch (error) {
    console.error('Error closing session:', error)
  }
}

async function sendMessage() {
  if (!newMessage.value.trim() || !selectedCustomer.value) return
  const message = {
    fromUserId: adminInfo.adminId,
    fromUserName: adminInfo.adminName,
    toUserId: selectedCustomer.value.userId,
    message: newMessage.value,
    timestamp: new Date(),
    isFromAdmin: true,
    messageId: generateMessageId()
  }
  const sessionKey = selectedCustomer.value.sessionId || selectedCustomer.value.userId
  if (!messages[sessionKey]) {
    messages[sessionKey] = []
  }
  messages[sessionKey].push(message)
  selectedCustomer.value.lastMessage = newMessage.value
  selectedCustomer.value.lastMessageTime = new Date()
  try {
    await safeInvoke('SendMessageToCustomer', selectedCustomer.value.userId, newMessage.value)
    newMessage.value = ''
    scrollToBottom()
  } catch (error) {
    console.error('Error sending message:', error)
    const messageIndex = messages[sessionKey].findIndex(m => m.messageId === message.messageId)
    if (messageIndex !== -1) {
      messages[sessionKey].splice(messageIndex, 1)
    }
  }
}

async function assignCustomerToMe() {
  if (!selectedCustomer.value) return
  try {
    await safeInvoke('AssignCustomerToMe', selectedCustomer.value.userId)
    selectedCustomer.value.isAssigned = true
    selectedCustomer.value.adminId = adminInfo.adminId
    selectedCustomer.value.adminName = adminInfo.adminName
  } catch (error) {
    console.error('Error assigning customer:', error)
  }
}

async function refreshHistory() {
  if (!selectedCustomer.value) return
  currentPage.value = 1
  const sessionKey = selectedCustomer.value.sessionId || selectedCustomer.value.userId
  messages[sessionKey] = []
  isLoadingHistory.value = true
  await loadChatHistory(selectedCustomer.value)
}

async function refreshSessions() {
  isLoading.value = true
  connectionReady.value = false
  await loadInitialData()
}

function setFilter(filter) {
  currentFilter.value = filter
}

function scrollToBottom() {
  nextTick(() => {
    const container = document.getElementById('messagesContainer')
    if (container) {
      container.scrollTop = container.scrollHeight
    }
  })
}
function formatTime(timestamp, isLastMessage = false, allowJustNow = true) {
  let date = new Date(timestamp)
  
  const now = new Date()
  const diff = now.getTime() - date.getTime() // Luôn là số dương

  if (allowJustNow && diff < 60000) return 'Vừa xong'
  if (diff < 3600000) return `${Math.floor(diff / 60000)} phút`
  if (diff < 86400000) return `${Math.floor(diff / 3600000)} giờ`
  if (diff < 604800000) return `${Math.floor(diff / 86400000)} ngày`
  return date.toLocaleDateString('vi-VN')
}
function getStatusText(status) {
  const statusMap = {
    'Active': 'Đang hoạt động',
    'Waiting': 'Đang chờ',
    'Assigned': 'Đã được phân công',
    'Closed': 'Đã đóng',
    'Abandoned': 'Đã rời đi'
  }
  return statusMap[status] || status
}

function generateMessageId() {
  return Date.now().toString() + Math.random().toString(36).substr(2, 9)
}

function getAuthToken() {
  const token = localStorage.getItem('accessToken')
  if (!token) {
    router.push('/login')
  }
  return token
}

function startCall() {
  console.log('Starting call with', selectedCustomer.value?.userName)
}

function startVideoCall() {
  console.log('Starting video call with', selectedCustomer.value?.userName)
}

function showInfo() {
  console.log('Showing info for', selectedCustomer.value?.userName)
}

function handleSearch() {
  // Logic for handling search can be implemented here if needed
}

function clearSearch() {
  searchQuery.value = ''
}
</script>

<style scoped>
.chat-container {
  display: flex;
  height: 85vh;
  /* max-width: 1400px; */
  margin: 0 auto;
  padding: 20px;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background: white;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.07);
  border: 1px solid #476f8c;
  border-radius: 12px;
}

.sidebar {
  width: 360px;
  background: white;
  border-right: 1px solid #e1e8ed;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

.sidebar-header {
  padding: 20px 25px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.section-title {
  margin: 0;
  font-size: 1.4rem;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 10px;
}

.search-container {
  padding: 20px 25px;
  position: relative;
}

.search-box {
  display: flex;
  align-items: center;
}

.search-input {
  width: 100%;
  padding: 12px 45px 12px 15px;
  border: 2px solid #e1e8ed;
  border-radius: 25px;
  font-size: 1rem;
  background: #fafbfc;
  color: #2c3e50;
  transition: all 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: #3498db;
  background: white;
  box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.1);
}

.search-input::placeholder {
  color: rgba(67, 63, 63, 0.7);
}

.search-icon {
  position: absolute;
  right: 40px;
  top: 50%;
  transform: translateY(-50%);
  color: rgba(13, 13, 13, 0.8);
  font-size: 1.1rem;
  cursor: pointer;
}

.clear-search {
  position: absolute;
  right: 60px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  color: rgba(13, 13, 13, 0.8);
  cursor: pointer;
  padding: 5px;
  border-radius: 50%;
  transition: all 0.3s ease;
}

.clear-search:hover {
  background: rgba(0, 0, 0, 0.1);
}

.filters {
  display: flex;
  gap: 10px;
  padding: 0 25px 20px;
}

.btn {
  padding: 12px 24px;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-outline-primary {

  background: transparent;
  color: #3498db;
  border: 2px solid #3498db;
}
.filter-btn{
  width: 23%;
  height: 50px;
  font-size: 13px;
}

.btn-outline-primary:hover:not(:disabled) {
  background: #3498db;
  color: white;
}

.btn-outline-primary.active {
  background: #3498db;
  color: white;
}

.btn-success {
  background: linear-gradient(135deg, #27ae60, #229954);
  color: white;
}

.btn-success:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(39, 174, 96, 0.4);
}

.btn-danger {
  background: linear-gradient(135deg, #e74c3c, #c0392b);
  color: white;
}

.btn-danger:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(231, 76, 60, 0.4);
}

.btn-sm {
  padding: 8px 12px;
  font-size: 0.85rem;
}

.loading-state {
  text-align: center;
  padding: 60px 20px;
  color: #7f8c8d;
  font-size: 1.1rem;
}

.loading-state i {
  font-size: 2rem;
  margin-bottom: 15px;
  display: block;
}

.customer-list {
  flex: 1;
  overflow-y: auto;
  min-height: 0;
  background: white;
}

.customer-item {
  display: flex;
  align-items: center;
  padding: 15px 25px;
  cursor: pointer;
  border-bottom: 1px solid #ecf0f1;
  transition: background 0.2s;
}

.customer-item:hover {
  background: #f8f9fa;
}

.customer-item.active {
  background: #f1f5f9;
}

.customer-info {
  flex: 1;
  min-width: 0;
}

.customer-name {
  font-size: 15px;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 4px;
}

.last-message {
  font-size: 13px;
  color: #7f8c8d;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.message-icon {
  margin-right: 5px;
}

.session-info {
  font-size: 12px;
  color: #7f8c8d;
}

.message-count {
  font-weight: 500;
}

.customer-meta {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 4px;
}

.time {
  font-size: 12px;
  color: #7f8c8d;
}

.unread-count {
  background: linear-gradient(135deg, #3498db, #2980b9);
  color: white;
  font-size: 12px;
  font-weight: 600;
  padding: 2px 6px;
  border-radius: 10px;
  min-width: 18px;
  text-align: center;
}

.offline-indicator i {
  color: #ccc;
  font-size: 10px;
}

.empty-state {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #7f8c8d;
  font-size: 1.1rem;
}

.empty-state i {
  font-size: 4rem;
  margin-bottom: 20px;
  display: block;
  color: #bdc3c7;
}

.chat-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: white;
  min-width: 0;
  min-height: 0;
}

.chat-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

.chat-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 25px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.chat-header .customer-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.customer-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
}

.chat-header .customer-info h3 {
  margin: 0;
  font-size: 1.3rem;
  font-weight: 600;
}

.status-info .status {
  font-size: 0.95rem;
}

.status.online {
  color: #27ae60;
}

.chat-actions {
  display: flex;
  gap: 8px;
}

.messages-container {
  flex: 1;
  min-height: 0;
  height: 100%;
  overflow-y: auto;
  padding: 25px;
  background: white;
}

.message-wrapper {
  margin-bottom: 15px;
}

.message {
  display: flex;
  flex-direction: column;
  max-width: 70%;
}

.message-sent {
  align-items: flex-end;
  margin-left: auto;
}

.message-received {
  align-items: flex-start;
}

.message-content {
  padding: 12px 15px;
  border-radius: 8px;
  background: #f1f5f9;
  color: #2c3e50;
}

.message-sent .message-content {
  background: linear-gradient(135deg, #3498db, #2980b9);
  color: white;
}

.message-content p {
  margin: 0;
  font-size: 1rem;
  line-height: 1.4;
}

.message-time {
  font-size: 0.85rem;
  color: #7f8c8d;
  padding: 0 12px;
  margin-top: 4px;
}

.read-status, .sent-status {
  margin-left: 5px;
}

.read-status {
  color: #27ae60;
}

.sent-status {
  color: #95a5a6;
}

.load-more-container {
  text-align: center;
  padding: 20px;
}

.input-area {
  padding: 25px;
  border-top: 1px solid #e1e8ed;
  background: white;
}

.input-container {
  display: flex;
  align-items: center;
  gap: 15px;
}

.message-input {
  flex: 1;
  display: flex;
  align-items: center;
  border: 2px solid #e1e8ed;
  border-radius: 8px;
  padding: 8px 12px;
  background: #fafbfc;
}

.message-input input {
  flex: 1;
  border: none;
  background: transparent;
  color: #2c3e50;
  font-size: 1rem;
  outline: none;
}

.message-input input::placeholder {
  color: #7f8c8d;
}

.closed-notice {
  margin: 20px 25px;
  color: #e74c3c;
  background: #fef2f2;
  border: 1px solid #fecaca;
  border-radius: 8px;
  padding: 12px 15px;
  font-size: 0.95rem;
  display: flex;
  align-items: center;
  gap: 10px;
}

.customer-list::-webkit-scrollbar,
.messages-container::-webkit-scrollbar {
  width: 4px;
}

.customer-list::-webkit-scrollbar-track,
.messages-container::-webkit-scrollbar-track {
  background: transparent;
}

.customer-list::-webkit-scrollbar-thumb,
.messages-container::-webkit-thumb {
  background: #e1e8ed;
  border-radius: 2px;
}

.customer-list::-webkit-scrollbar-thumb:hover,
.messages-container::-webkit-scrollbar-thumb:hover {
  background: #d1d5db;
}

/* Responsive Design */
@media (max-width: 768px) {
  .chat-container {
    padding: 15px;
  }

  .sidebar {
    width: 100%;
    max-width: 300px;
  }

  .section-title {
    font-size: 1.2rem;
  }

  .filters {
    flex-direction: column;
    gap: 8px;
  }

  .customer-item {
    padding: 10px 15px;
  }

  .chat-header {
    flex-direction: column;
    gap: 15px;
    align-items: flex-start;
  }

  .messages-container {
    padding: 15px;
  }

  .input-container {
    flex-wrap: wrap;
  }

  .message-input {
    width: 100%;
  }
}

@media (max-width: 480px) {
  .customer-item {
    font-size: 0.9rem;
  }

  .customer-name {
    font-size: 0.9rem;
  }

  .last-message {
    font-size: 0.8rem;
  }

  .btn {
    padding: 10px 16px;
    font-size: 0.9rem;
  }

  .btn-sm {
    padding: 6px 10px;
    font-size: 0.8rem;
  }
}
</style>