import Swal from 'sweetalert2'

// Cấu hình mặc định cho tất cả thông báo
const defaultConfig = {
  timer: 3000,
  timerProgressBar: true,
  showConfirmButton: true,
  allowOutsideClick: true,
  allowEscapeKey: true,
  position: 'center',
  toast: false,
  background: '#fff',
  customClass: {
    popup: 'animated fadeInDown'
  }
}

// Cấu hình cho từng loại thông báo
const typeConfigs = {
  success: {
    icon: 'success',
    iconColor: '#28a745',
    confirmButtonColor: '#28a745',
    title: 'Thành công!',
    background: '#f8fff9'
  },
  error: {
    icon: 'error',
    iconColor: '#dc3545',
    confirmButtonColor: '#dc3545',
    title: 'Lỗi!',
    background: '#fff5f5'
  },
  warning: {
    icon: 'warning',
    iconColor: '#ffc107',
    confirmButtonColor: '#ffc107',
    title: 'Cảnh báo!',
    background: '#fffbf0'
  },
  info: {
    icon: 'info',
    iconColor: '#17a2b8',
    confirmButtonColor: '#17a2b8',
    title: 'Thông tin!',
    background: '#f0f9ff'
  },
  question: {
    icon: 'question',
    iconColor: '#6f42c1',
    confirmButtonColor: '#6f42c1',
    title: 'Xác nhận?',
    background: '#f8f5ff'
  }
}

class NotificationService {
  constructor() {
    this.defaultTimer = 3000
  }

  // Hàm hiển thị thông báo chính
  show(options = {}) {
    const config = this._buildConfig(options)
    return Swal.fire(config)
  }

  // Thông báo thành công
  success(message, options = {}) {
    return this.show({
      type: 'success',
      text: message,
      ...options
    })
  }

  // Thông báo lỗi
  error(message, options = {}) {
    return this.show({
      type: 'error',
      text: message,
      timer: 5000, // Lỗi hiển thị lâu hơn
      ...options
    })
  }

  // Thông báo cảnh báo
  warning(message, options = {}) {
    return this.show({
      type: 'warning',
      text: message,
      ...options
    })
  }

  // Thông báo thông tin
  info(message, options = {}) {
    return this.show({
      type: 'info',
      text: message,
      ...options
    })
  }

  // Hộp thoại xác nhận
  confirm(message, options = {}) {
    return this.show({
      type: 'question',
      text: message,
      showCancelButton: true,
      confirmButtonText: 'Xác nhận',
      cancelButtonText: 'Hủy',
      timer: null, // Không tự đóng
      ...options
    })
  }

  // Thông báo dạng toast (góc màn hình)
  toast(type, message, options = {}) {
    return this.show({
      type,
      text: message,
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
      ...options
    })
  }

  // Toast thành công
  toastSuccess(message, options = {}) {
    return this.toast('success', message, options)
  }

  // Toast lỗi
  toastError(message, options = {}) {
    return this.toast('error', message, {
      timer: 4000,
      ...options
    })
  }

  // Toast cảnh báo
  toastWarning(message, options = {}) {
    return this.toast('warning', message, options)
  }

  // Toast thông tin
  toastInfo(message, options = {}) {
    return this.toast('info', message, options)
  }

  // Thông báo loading
  loading(title = 'Đang xử lý...', options = {}) {
    return Swal.fire({
      title,
      allowOutsideClick: false,
      allowEscapeKey: false,
      showConfirmButton: false,
      didOpen: () => {
        Swal.showLoading()
      },
      ...options
    })
  }

  // Đóng loading
  closeLoading() {
    Swal.close()
  }

  // Thông báo với input
  input(options = {}) {
    return Swal.fire({
      title: 'Nhập thông tin',
      input: 'text',
      inputPlaceholder: 'Nhập...',
      showCancelButton: true,
      confirmButtonText: 'Xác nhận',
      cancelButtonText: 'Hủy',
      inputValidator: (value) => {
        if (!value) {
          return 'Vui lòng nhập thông tin!'
        }
      },
      ...options
    })
  }

  // Thông báo với nhiều button tùy chỉnh
  custom(options = {}) {
    return Swal.fire({
      title: options.title || 'Thông báo',
      text: options.text || '',
      icon: options.icon || 'info',
      showCancelButton: true,
      showDenyButton: options.showThirdButton || false,
      confirmButtonText: options.confirmText || 'Đồng ý',
      cancelButtonText: options.cancelText || 'Hủy',
      denyButtonText: options.denyText || 'Không',
      ...options
    })
  }

  // Thông báo tiến trình
  progress(title = 'Đang xử lý...') {
    let timerInterval
    
    return Swal.fire({
      title,
      html: 'Còn lại <b></b> giây.',
      timer: 10000,
      timerProgressBar: true,
      allowOutsideClick: false,
      didOpen: () => {
        Swal.showLoading()
        const b = Swal.getHtmlContainer().querySelector('b')
        timerInterval = setInterval(() => {
          b.textContent = Math.ceil(Swal.getTimerLeft() / 1000)
        }, 100)
      },
      willClose: () => {
        clearInterval(timerInterval)
      }
    })
  }

  // Thông báo với HTML tùy chỉnh
  html(htmlContent, options = {}) {
    return Swal.fire({
      html: htmlContent,
      showCloseButton: true,
      showConfirmButton: false,
      focusConfirm: false,
      ...options
    })
  }

  // Thông báo với hình ảnh
  image(imageUrl, options = {}) {
    return Swal.fire({
      title: options.title || 'Hình ảnh',
      imageUrl,
      imageWidth: options.imageWidth || 400,
      imageHeight: options.imageHeight || 200,
      imageAlt: options.imageAlt || 'Custom image',
      ...options
    })
  }

  // Hàm private để build config
  _buildConfig(options) {
    const typeConfig = typeConfigs[options.type] || {}
    
    return {
      ...defaultConfig,
      ...typeConfig,
      title: options.title || typeConfig.title,
      text: options.text || options.message,
      timer: options.timer !== undefined ? options.timer : defaultConfig.timer,
      confirmButtonText: options.confirmButtonText || 'OK',
      cancelButtonText: options.cancelButtonText || 'Hủy',
      position: options.position || defaultConfig.position,
      toast: options.toast !== undefined ? options.toast : defaultConfig.toast,
      showConfirmButton: options.showConfirmButton !== undefined ? options.showConfirmButton : defaultConfig.showConfirmButton,
      showCancelButton: options.showCancelButton || false,
      allowOutsideClick: options.allowOutsideClick !== undefined ? options.allowOutsideClick : defaultConfig.allowOutsideClick,
      ...options
    }
  }
}

// Tạo instance duy nhất
const notification = new NotificationService()

// Export các hàm tiện ích
export const {
  show,
  success,
  error,
  warning,
  info,
  confirm,
  toast,
  toastSuccess,
  toastError,
  toastWarning,
  toastInfo,
  loading,
  closeLoading,
  input,
  custom,
  progress,
  html,
  image
} = notification

// Export default instance
export default notification

// ====================== EXAMPLES ======================

/* 
// Cài đặt SweetAlert2
npm install sweetalert2

// Import và sử dụng
import notification from '@/utils/notification'
// hoặc
import { success, error, confirm } from '@/utils/notification'

// 1. Thông báo cơ bản
notification.success('Đăng nhập thành công!')
notification.error('Có lỗi xảy ra!')
notification.warning('Cảnh báo: Dữ liệu không hợp lệ')
notification.info('Thông tin: Hệ thống sẽ bảo trì vào 2h sáng')

// 2. Thông báo với tùy chỉnh
notification.success('Lưu thành công!', {
  title: 'Hoàn tất',
  timer: 5000,
  position: 'center'
})

// 3. Xác nhận
const result = await notification.confirm('Bạn có chắc chắn muốn xóa?')
if (result.isConfirmed) {
  // Xử lý xóa
}

// 4. Toast notifications
notification.toastSuccess('Cập nhật thành công!')
notification.toastError('Kết nối thất bại!')

// 5. Loading
notification.loading('Đang tải dữ liệu...')
// Sau khi hoàn thành
notification.closeLoading()

// 6. Input dialog
const { value } = await notification.input({
  title: 'Nhập tên của bạn',
  inputPlaceholder: 'Tên...'
})

// 7. Custom buttons
const result = await notification.custom({
  title: 'Chọn hành động',
  text: 'Bạn muốn làm gì?',
  showThirdButton: true,
  confirmText: 'Lưu',
  cancelText: 'Hủy',
  denyText: 'Xóa'
})

// 8. HTML content
notification.html(`
  <div>
    <h3>Thông báo quan trọng</h3>
    <p>Nội dung <strong>HTML</strong> tùy chỉnh</p>
  </div>
`)

// 9. Với hình ảnh
notification.image('/path/to/image.jpg', {
  title: 'Ảnh đại diện mới'
})
*/