<template>
  <div class="employees-management">
    <!-- Add Employee Section -->
    <div class="card add-employee-section">
      <div class="card-header">
        <h2 class="section-title">
          <i class="fas fa-user-plus"></i>
          Thêm nhân viên mới
        </h2>
        <button 
          @click="toggleAddForm" 
          class="btn btn-primary"
          :class="{ 'active': showAddForm }"
        >
          <i class="fas" :class="showAddForm ? 'fa-minus' : 'fa-plus'"></i>
          {{ showAddForm ? 'Ẩn form' : 'Hiển thị form' }}
        </button>
      </div>
      
      <div v-if="showAddForm" class="card-body">
        <form @submit.prevent="createEmployee" class="employee-form">
          <div class="form-grid">

            <div class="form-group">
              <label for="lastName">Tên <span class="required">*</span></label>
              <input
                id="Name"
                v-model="newEmployee.Name"
                type="text"
                class="form-control"
                placeholder="Nhập tên "
                required
              />
            </div>

            <div class="form-group">
              <label for="email">Email <span class="required">*</span></label>
              <input
                id="email"
                v-model="newEmployee.Email"
                type="email"
                class="form-control"
                placeholder="Nhập email"
                required
              />
            </div>

            <div class="form-group">
              <label for="phone">Số điện thoại <span class="required">*</span></label>
              <input
                id="phone"
                v-model="newEmployee.PhoneNumber"
                type="tel"
                class="form-control"
                placeholder="Nhập số điện thoại"
                required
              />
            </div>

            <div class="form-group full-width">
              <label for="address">Địa chỉ <span class="required">*</span></label>
              <input
                id="address"
                v-model="newEmployee.Address"
                type="text"
                class="form-control"
                placeholder="Nhập địa chỉ"
                required
              />
            </div>

            <div class="form-group">
              <label for="role">Vai trò <span class="required">*</span></label>
              <select
                id="role"
                v-model="newEmployee.Role"
                class="form-control"
                required
              >
                <option value="">Chọn vai trò</option>
                <option value="0">Thu ngân</option>
                <option value="1">Quản lý kho</option>
              </select>
            </div>
          </div>

          <div class="form-actions">
            <button 
              type="submit" 
              class="btn btn-success"
              :disabled="loading"
            >
              <i class="fas fa-save"></i>
              {{ add_loading ? 'Đang tạo...' : 'Tạo nhân viên' }}
            </button>
            <button 
              type="button" 
              @click="resetForm" 
              class="btn btn-secondary"
            >
              <i class="fas fa-undo"></i>
              Reset
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Employees List Section -->
    <div class="card employees-list-section">
      <div class="card-header">
        <h2 class="section-title">
          <i class="fas fa-list"></i>
          Danh sách nhân viên
        </h2>
        <div class="list-controls">
          <div class="search-container">
                <input
                    v-model="searchQuery"
                    type="text"
                    class="search-input"
                    placeholder="Tìm tên nhân viên"
                />
                <button
                  v-if="searchQuery"
                  @click="clearSearch"
                  class="clear-search"
                  title="Xóa tìm kiếm"
                >
                  <i class="fas fa-times"></i>
                </button>
                <i @click="handleSearch" class="fas fa-search search-icon"></i>
            </div>
          <button @click="refreshList" class="btn btn-outline-primary">
            <i class="fas fa-sync-alt"></i>
            Làm mới
          </button>
        </div>
      </div>

      <div class="card-body">
        <!-- Loading State -->
        <div v-if="loading" class="loading-state">
          <i class="fas fa-spinner fa-spin"></i>
          Đang tải...
        </div>

        <!-- Empty State -->
        <div v-else-if="!employees.length" class="empty-state">
          <i class="fas fa-users-slash"></i>
          <p>Không có nhân viên nào</p>
        </div>

        <!-- Employees Table -->
        <div v-else class="table-responsive">
          <table class="employees-table">
            <thead>
              <tr>
                <th>Họ tên</th>
                <th>Email</th>
                <th>Số điện thoại</th>
                <th>Địa chỉ</th>
                <th>Vai trò</th>
                <th>Trạng thái</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="employee in employees" :key="employee.id" class="employee-row">
                <td class="employee-name">
                  <div class="name-container">
                    {{ employee.name }}
                  </div>
                </td>
                <td class="employee-email">
                  <a :href="`mailto:${employee.email}`" class="email-link">
                    {{ employee.email }}
                  </a>
                </td>
                <td class="employee-phone">
                  <a :href="`tel:${employee.phone}`" class="phone-link">
                    {{ employee.phone }}
                  </a>
                </td>
                <td class="employee-address">{{ employee.address }}</td>
                <td class="employee-role">
                  <span class="role-badge" :class="getRoleClass(employee.role)">
                    {{ getRoleText(employee.role) }}
                  </span>
                </td>
                <td class="employee-status">
                  <label class="status-switch">
                    <input
                      type="checkbox"
                      :checked="employee.status"
                      @change="updateEmployeeStatus(employee.id, $event.target.checked)"
                      :disabled="updatingStatusIds.includes(employee.id)"
                    />
                    <span class="slider" :class="{ 'updating': updatingStatusIds.includes(employee.id) }"></span>
                  </label>
                </td>
                <td class="employee-actions">
                  <div class="action-buttons">
                    <button 
                      @click="openEditModal(employee)" 
                      class="btn btn-sm btn-info"
                      title="Chỉnh sửa"
                    >
                      <i class="fas fa-edit"></i>
                    </button>
                    <select
                      :value="employee.role"
                      @change="updateEmployeeRole(employee.id, $event.target.value)"
                      class="role-select"
                      title="Thay đổi vai trò"
                      :disabled="updatingRoleIds.includes(employee.id)"
                    >
                      <option value="InventoryManager">Quản lý kho</option>
                      <option value="Cashier">Thu ngân</option>
                    </select>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        <div v-if="employees.length" class="pagination-container">
          <button 
            @click="previousPage" 
            :disabled="currentPage <= 1" 
            class="btn btn-outline-primary"
          >
            <i class="fas fa-chevron-left"></i>
            Trước
          </button>
          <span class="page-info">Trang {{ currentPage }}</span>
          <button 
            :disabled="currentPage >= maxPage"
            @click="nextPage" 
            class="btn btn-outline-primary"
          >
            Sau
            <i class="fas fa-chevron-right"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- Edit Modal -->
    <div v-if="showEditModal" class="modal-overlay" @click="closeEditModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>Chỉnh sửa nhân viên</h3>
          <button @click="closeEditModal" class="close-btn">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="updateEmployee" class="employee-form">
            <div class="form-grid">
              <div class="form-group">
                <label>Tên</label>
                <input
                  v-model="editingEmployee.Name"
                  type="text"
                  class="form-control"
                  required
                />
              </div>
              <div class="form-group">
                <label>Email</label>
                <input
                  v-model="editingEmployee.Email"
                  type="email"
                  class="form-control"
                  required
                />
              </div>
              <div class="form-group">
                <label>Số điện thoại</label>
                <input
                  v-model="editingEmployee.PhoneNumber"
                  type="tel"
                  class="form-control"
                  required
                />
              </div>
              <div class="form-group full-width">
                <label>Địa chỉ</label>
                <input
                  v-model="editingEmployee.Address"
                  type="text"
                  class="form-control"
                  required
                />
              </div>
            </div>
            <div class="modal-actions">
              <button type="submit" class="btn btn-success" :disabled="loading">
                <i class="fas fa-save"></i>
                {{ loading ? 'Đang lưu...' : 'Lưu thay đổi' }}
              </button>
              <button type="button" @click="closeEditModal" class="btn btn-secondary">
                <i class="fas fa-times"></i>
                Hủy
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Toast Notifications -->
    <div class="toast-container">
      <div 
        v-for="toast in toasts" 
        :key="toast.id" 
        class="toast" 
        :class="toast.type"
      >
        <i class="fas" :class="getToastIcon(toast.type)"></i>
        {{ toast.message }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, reactive } from 'vue'
import EmployeeService from '../services/employeeService'

    // Reactive data
    const employees = ref([])
    const loading = ref(false)
    const showAddForm = ref(false)
    const showEditModal = ref(false)
    const currentPage = ref(1)
    const maxPage = ref() 
    const toasts = ref([])
    const searchQuery = ref('')
    const add_loading =  ref(false)
    // Tracking arrays for optimistic updates
    const updatingStatusIds = ref([])
    const updatingRoleIds = ref([])

    // New employee form
    const newEmployee = reactive({
      Email: '',
      Name: '',
      PhoneNumber: '',
      Address: '',
      Role: ''
    })

    // Editing employee form
    const editingEmployee = reactive({
      EmpId: '',
      Email: '',
      Name: '',
      PhoneNumber: '',
      Address: ''
    })

    // Methods
    const showToast = (message, type = 'info') => {
      const toast = {
        id: Date.now(),
        message,
        type
      }
      toasts.value.push(toast)
      setTimeout(() => {
        const index = toasts.value.findIndex(t => t.id === toast.id)
        if (index > -1) toasts.value.splice(index, 1)
      }, 3000)
    }

    const getToastIcon = (type) => {
      switch (type) {
        case 'success': return 'fa-check-circle'
        case 'error': return 'fa-exclamation-circle'
        case 'warning': return 'fa-exclamation-triangle'
        default: return 'fa-info-circle'
      }
    }

    const toggleAddForm = () => {
      showAddForm.value = !showAddForm.value
    }

    const resetForm = () => {
      Object.keys(newEmployee).forEach(key => {
        newEmployee[key] = ''
      })
    }

    const handleSearch = async () => {
    if (searchQuery.value.trim() !== '') {
      currentPage.value = 1
      const response = await EmployeeService.searchEmployees(searchQuery.value, currentPage.value)
      employees.value = response.data
      maxPage.value = response.maxPage
    }
  }
    const clearSearch = async () => {
    searchQuery.value = ''
    currentPage.value = 1
    await loadEmployees()
  }

  const refreshList = async () => {
  searchQuery.value = ''   // xoá keyword
  currentPage.value = 1    // quay về trang 1
  await loadEmployees()    // tải full danh sách
}

    const loadEmployees = async () => {
      try {
        loading.value = true
        const response = await EmployeeService.getEmployees(currentPage.value)
        console.log('Loaded employees:', response)

        employees.value = response.data
        maxPage.value = response.maxPage

      } catch (error) {
        showToast('Lỗi khi tải danh sách nhân viên', 'error')
        console.error('Error loading employees:', error)
      } finally {
        loading.value = false
      }
    }

    const createEmployee = async () => {
      try {
        add_loading.value =true
        await EmployeeService.createEmployee(newEmployee)
        showToast('Tạo nhân viên thành công!', 'success')
        resetForm()
        showAddForm.value = false
        
      } catch (error) {
        showToast(error.message, 'error')
        console.error('Error creating employee:', error)
      } finally {
        add_loading.value = false
      }
    }

    const updateEmployee = async () => {
      try {
        loading.value = true
        await EmployeeService.updateEmployee(editingEmployee)
        showToast('Cập nhật nhân viên thành công!', 'success')
        closeEditModal()
        await loadEmployees()
      } catch (error) {
        showToast(error.message, 'error')
        console.error('Error updating employee:', error)
      } finally {
        loading.value = false
      }
    }

    // Optimized status update with immediate UI feedback
    const updateEmployeeStatus = async (empId, newStatus) => {
      // Find employee and store old status
      const employee = employees.value.find(emp => emp.id === empId)
      if (!employee) return
      
      const oldStatus = employee.status
      
      // Add to updating list and update UI immediately
      updatingStatusIds.value.push(empId)
      employee.status = newStatus
      
      try {
        await EmployeeService.updateEmployeeStatus(empId, newStatus)
        showToast(`Trạng thái nhân viên đã được ${newStatus ? 'kích hoạt' : 'vô hiệu hóa'}`, 'success')
      } catch (error) {
        // Revert UI change on error
        employee.status = oldStatus
        showToast(error.message, 'error')
        console.error('Error updating status:', error)
      } finally {
        // Remove from updating list
        const index = updatingStatusIds.value.indexOf(empId)
        if (index > -1) {
          updatingStatusIds.value.splice(index, 1)
        }
      }
    }

    // Optimized role update with immediate UI feedback
    const updateEmployeeRole = async (empId, newRole) => {
      // Find employee and store old role
      const employee = employees.value.find(emp => emp.id === empId)
      if (!employee) return
      
      const oldRole = employee.role
      
      // Add to updating list and update UI immediately
      updatingRoleIds.value.push(empId)
      employee.role = newRole
      
      try {
        const roleValue = newRole === 'Cashier' ? 0 : 1
        await EmployeeService.updateEmployeeRole(empId, roleValue)
        showToast('Vai trò nhân viên đã được cập nhật', 'success')
      } catch (error) {
        // Revert UI change on error
        employee.role = oldRole
        showToast(error.message, 'error')
        console.error('Error updating role:', error)
      } finally {
        // Remove from updating list
        const index = updatingRoleIds.value.indexOf(empId)
        if (index > -1) {
          updatingRoleIds.value.splice(index, 1)
        }
      }
    }

    const deleteEmployee = async (empId) => {
      if (confirm('Bạn có chắc chắn muốn xóa nhân viên này?')) {
        try {
          await EmployeeService.deleteEmployee(empId)
          showToast('Xóa nhân viên thành công!', 'success')
          await loadEmployees()
        } catch (error) {
          showToast('Lỗi khi xóa nhân viên', 'error')
          console.error('Error deleting employee:', error)
        }
      }
    }

    const openEditModal = (employee) => {
      editingEmployee.EmpId = employee.id
      editingEmployee.Email = employee.email
      editingEmployee.Name = employee.name
      editingEmployee.PhoneNumber = employee.phone
      editingEmployee.Address = employee.address
      showEditModal.value = true
    }

    const closeEditModal = () => {
      showEditModal.value = false
      Object.keys(editingEmployee).forEach(key => {
        editingEmployee[key] = ''
      })
    }

    const getRoleClass = (role) => {
      return role === 'Cashier' ? 'cashier' : 'inventory-manager'
    }

    const getRoleText = (role) => {
      return role === 'Cashier' ? 'Thu ngân' : 'Quản lý kho'
    }

    const previousPage = async () => {
  if (currentPage.value > 1) {
    currentPage.value--
    if (searchQuery.value.trim() !== '') {
      const response = await EmployeeService.searchEmployees(searchQuery.value, currentPage.value)
      employees.value = response.data
      maxPage.value = response.maxPage
    } else {
      await loadEmployees()
    }
  }
}


    const nextPage = async () => {
  if (currentPage.value < maxPage.value) {
    currentPage.value++
    if (searchQuery.value.trim() !== '') {
      const response = await EmployeeService.searchEmployees(searchQuery.value, currentPage.value)
      employees.value = response.data
      maxPage.value = response.maxPage
    } else {
      await loadEmployees()
    }
  }
}

    // Lifecycle
    onMounted(() => {
      loadEmployees()
    })
  
</script>   

<style scoped>
.employees-management {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.header {
  margin-bottom: 30px;
}

.title {
  color: #2c3e50;
  font-size: 2.5rem;
  font-weight: 600;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 15px;
}

.title i {
  color: #3498db;
}

.card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.07);
  margin-bottom: 30px;
  overflow: hidden;
  border: 1px solid #e1e8ed;
}

.card-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 20px 25px;
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

.card-body {
  padding: 25px;
}

.employee-form {
  max-width: none;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 25px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group.full-width {
  grid-column: 1 / -1;
}

.form-group label {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 8px;
  font-size: 0.95rem;
}

.required {
  color: #e74c3c;
}

.form-control {
  padding: 12px 15px;
  border: 2px solid #e1e8ed;
  border-radius: 8px;
  font-size: 1rem;
  transition: all 0.3s ease;
  background: #fafbfc;
}

.form-control:focus {
  outline: none;
  border-color: #3498db;
  background: white;
  box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.1);
}

.form-actions {
  display: flex;
  gap: 15px;
  justify-content: flex-start;
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
  text-decoration: none;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background: linear-gradient(135deg, #3498db, #2980b9);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(52, 152, 219, 0.4);
}

.btn-primary.active {
  background: linear-gradient(135deg, #e74c3c, #c0392b);
}

.btn-success {
  background: linear-gradient(135deg, #27ae60, #229954);
  color: white;
}

.btn-success:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(39, 174, 96, 0.4);
}

.btn-secondary {
  background: linear-gradient(135deg, #95a5a6, #7f8c8d);
  color: white;
}

.btn-warning {
  background: linear-gradient(135deg, #f39c12, #e67e22);
  color: white;
}

.btn-danger {
  background: linear-gradient(135deg, #e74c3c, #c0392b);
  color: white;
}

.btn-outline-primary {
  background: transparent;
  color: #3498db;
  border: 2px solid #3498db;
}

.btn-outline-primary:hover:not(:disabled) {
  background: #3498db;
  color: white;
}

.list-controls {
  display: flex;
  gap: 10px;
}
/* Search Bar Styles */
        .search-container {
          position: relative;
          flex: 1;
          max-width: 400px;
        }

        .search-input {
          width: 100%;
          padding: 12px 45px 12px 15px;
          border: 2px solid #e1e8ed;
          border-radius: 25px;
          font-size: 1rem;
          background: rgba(255, 255, 255, 0.9);
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
          right: 15px;
          top: 50%;
          transform: translateY(-50%);
          color: rgba(13, 13, 13, 0.8);
          font-size: 1.1rem;
        }

        .clear-search {
          position: absolute;
          right: 40px;
          top: 50%;
          transform: translateY(-50%);
          background: none;
          border: none;
          color: rgba(255, 255, 255, 0.8);
          cursor: pointer;
          padding: 5px;
          border-radius: 50%;
          transition: all 0.3s ease;
          display: none;
        }

        .clear-search:hover {
          background: rgba(255, 255, 255, 0.2);
          color: white;
        }

        .clear-search.show {
          display: block;
        }

        .search-results-info {
          margin: 15px 0;
          padding: 10px 15px;
          background: linear-gradient(135deg, #e8f4fd, #d1ecf1);
          border-radius: 8px;
          color: #2c3e50;
          font-size: 0.95rem;
          border-left: 4px solid #3498db;
        }

.loading-state, .empty-state {
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

.empty-state i {
  font-size: 4rem;
  margin-bottom: 20px;
  display: block;
  color: #bdc3c7;
}

.table-responsive {
  overflow-x: auto;
}

.employees-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

.employees-table th {
  background: linear-gradient(135deg, #34495e, #2c3e50);
  color: white;
  padding: 15px 12px;
  text-align: left;
  font-weight: 600;
  font-size: 0.95rem;
  border-bottom: 3px solid #3498db;
}

.employees-table td {
  padding: 15px 12px;
  border-bottom: 1px solid #ecf0f1;
  vertical-align: middle;
}

.employee-row:hover {
  background: #f8f9fa;
}

.employee-id {
  font-family: 'Courier New', monospace;
  font-weight: 600;
  color: #7f8c8d;
  font-size: 0.9rem;
}

.name-container {
  display: flex;
  align-items: center;
  gap: 10px;
}

.name-container i {
  color: #3498db;
  font-size: 1.2rem;
}

.email-link, .phone-link {
  color: #3498db;
  text-decoration: none;
}

.email-link:hover, .phone-link:hover {
  text-decoration: underline;
}

.role-badge {
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.role-badge.cashier {
  background: linear-gradient(135deg, #f39c12, #e67e22);
  color: white;
}

.role-badge.inventory-manager {
  background: linear-gradient(135deg, #9b59b6, #8e44ad);
  color: white;
}

.status-switch {
  position: relative;
  display: inline-block;
  width: 50px;
  height: 24px;
}

.status-switch input {
  opacity: 0;
  width: 0;
  height: 0;
}

.slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #ccc;
  transition: .4s;
  border-radius: 24px;
}

.slider:before {
  position: absolute;
  content: "";
  height: 18px;
  width: 18px;
  left: 3px;
  bottom: 3px;
  background-color: white;
  transition: .4s;
  border-radius: 50%;
}

input:checked + .slider {
  background: linear-gradient(135deg, #27ae60, #229954);
}

input:checked + .slider:before {
  transform: translateX(26px);
}

.action-buttons {
  display: flex;
  gap: 8px;
  align-items: center;
}

.btn-sm {
  padding: 8px 12px;
  font-size: 0.85rem;
}

.role-select {
  padding: 6px 10px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 0.85rem;
  background: white;
}

.pagination-container {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  margin-top: 30px;
  padding-top: 20px;
  border-top: 1px solid #ecf0f1;
}

.page-info {
  font-weight: 600;
  color: #2c3e50;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 12px;
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

.modal-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 20px 25px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h3 {
  margin: 0;
  font-size: 1.3rem;
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 1.5rem;
  cursor: pointer;
  padding: 5px;
  border-radius: 4px;
  transition: background 0.3s ease;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.2);
}

.modal-body {
  padding: 25px;
}

.modal-actions {
  display: flex;
  gap: 15px;
  justify-content: flex-end;
  margin-top: 25px;
}

/* Toast Notifications */
.toast-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 1100;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.toast {
  padding: 15px 20px;
  border-radius: 8px;
  color: white;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 10px;
  min-width: 250px;
  animation: slideIn 0.3s ease;
}

.toast.success {
  background: linear-gradient(135deg, #27ae60, #229954);
}

.toast.error {
  background: linear-gradient(135deg, #e74c3c, #c0392b);
}

.toast.warning {
  background: linear-gradient(135deg, #f39c12, #e67e22);
}

.toast.info {
  background: linear-gradient(135deg, #3498db, #2980b9);
}

@keyframes slideIn {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

/* Responsive Design */
@media (max-width: 768px) {
  .employees-management {
    padding: 15px;
  }

  .title {
    font-size: 2rem;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .card-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }

  .employees-table {
    font-size: 0.9rem;
  }

  .employees-table th,
  .employees-table td {
    padding: 10px 8px;
  }

  .action-buttons {
    flex-direction: column;
    gap: 5px;
  }

  .modal-content {
    width: 95%;
    margin: 10px;
  }

  .toast-container {
    left: 10px;
    right: 10px;
  }

  .toast {
    min-width: auto;
  }
}

@media (max-width: 480px) {
  .table-responsive {
    font-size: 0.8rem;
  }
  
  .employees-table th,
  .employees-table td {
    padding: 8px 6px;
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