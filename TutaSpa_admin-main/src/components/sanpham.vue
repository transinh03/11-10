<template>
  <div class="product-management">
    <!-- Header -->
    <div class="header">
      <h1 class="title">
        <i class="fas fa-box"></i>
        Quản lý sản phẩm
      </h1>
    </div>

    <!-- Filters and Search Section -->
    <div class="card filters-section">
      <div class="card-header">
        <h2 class="section-title">
          <i class="fas fa-filter"></i>
          Tìm kiếm & Lọc sản phẩm
        </h2>
        <div class="list-controls">
          <button class="btn btn-outline-primary" @click="resetAndRefresh">
            <i class="fas fa-sync-alt"></i>
            Làm mới
          </button>
        </div>
      </div>
      <div class="card-body">
        <div class="form-grid">
          <!-- Search by name -->
          <div class="form-group">
            <label class="form-label">
              <i class="fas fa-search"></i> Tìm theo tên sản phẩm
            </label>
            <div class="search-container">
              <input
                v-model="searchName"
                type="text"
                class="search-input"
                placeholder="Nhập tên sản phẩm..."
                @input="debounceSearch"
              />
              <i class="fas fa-search search-icon"></i>
              <button
                v-if="searchName"
                class="clear-search show"
                @click="clearSearch"
                type="button"
              >
                <i class="fas fa-times"></i>
              </button>
            </div>
          </div>

          <!-- Price filter -->
          <div class="form-group">
            <label class="form-label">
              <i class="fas fa-dollar-sign"></i> Lọc theo giá
            </label>
            <div class="price-filter-group">
              <input
                v-model.number="priceMin"
                type="number"
                class="form-control price-input"
                placeholder="Giá thấp nhất"
                min="0"
                @input="debounceFilter"  
              />
              <span class="price-separator">-</span>
              <input
                v-model.number="priceMax"
                type="number"
                class="form-control price-input"
                placeholder="Giá cao nhất"
                min="0"
                @input="debounceFilter"   
              />
            </div>
          </div>

          <!-- Category filter -->
          <div class="form-group">
            <label class="form-label">
              <i class="fas fa-tags"></i> Lọc theo danh mục
            </label>
            <select
              v-model="selectedCategory"
              @change="filterByCategory"
              class="form-control"
            >
              <option value="">Tất cả danh mục</option>
              <option v-for="cat in categories" :key="cat.loaiSanPhamId" :value="cat.loaiSanPhamId">
                {{ cat.tenLoai }}
              </option>
            </select>
          </div>

          <!-- Filter status display -->
          <div class="form-group">
            <label class="form-label">
              <i class="fas fa-info-circle"></i> Trạng thái lọc
            </label>
            <div class="filter-status">
              <span v-if="hasActiveFilters" class="filter-badge active">
                <i class="fas fa-filter"></i>
                Đang lọc ({{ totalItems }} kết quả)
              </span>
              <span v-else class="filter-badge">
                <i class="fas fa-list"></i>
                Hiển thị tất cả ({{ totalItems }} sản phẩm)
              </span>
            </div>
          </div>
        </div>

        <!-- Active filters display -->
        <div v-if="hasActiveFilters" class="active-filters">
          <h4>Bộ lọc đang áp dụng:</h4>
          <div class="filter-tags">
            <span v-if="searchName" class="filter-tag">
              <i class="fas fa-search"></i>
              Tên: "{{ searchName }}"
              <button @click="clearSearchFilter" class="remove-filter">×</button>
            </span>
            <span v-if="priceMin || priceMax" class="filter-tag">
              <i class="fas fa-dollar-sign"></i>
              Giá: {{ formatPriceRange() }}
              <button @click="clearPriceFilter" class="remove-filter">×</button>
            </span>
            <span v-if="selectedCategory" class="filter-tag">
              <i class="fas fa-tag"></i>
              Danh mục: {{ getCategoryName(selectedCategory) }}
              <button @click="clearCategoryFilter" class="remove-filter">×</button>
            </span>
          </div>
        </div>
      </div>
    </div>

    <div class="main-content">
      <!-- Edit Product Section -->
      <div class="card edit-product-section" v-if="showEditForm">
        <div class="card-header">
          <h2 class="section-title">
            <i class="fas fa-edit"></i>
            Cập nhật sản phẩm
          </h2>
          <button
            @click="toggleEditForm"
            class="btn btn-primary"
          >
            <i class="fas fa-minus"></i>
            Ẩn form
          </button>
        </div>

        <div class="card-body">
          <form @submit.prevent="saveProduct" class="product-form" enctype="multipart/form-data">
            <div class="form-grid">
              <div class="form-group">
                <label class="form-label">
                  <i class="fas fa-tag"></i> Tên sản phẩm
                  <span class="required">*</span>
                </label>
                <input
                  v-model="product.productName"
                  type="text"
                  class="form-control"
                  placeholder="Nhập tên sản phẩm"
                  required
                />
              </div>

              <div class="form-group">
                <label class="form-label">
                  <i class="fas fa-money-bill-wave"></i> Giá
                  <span class="required">*</span>
                </label>
                <input
                  v-model.number="product.price"
                  type="number"
                  class="form-control"
                  placeholder="Nhập giá sản phẩm"
                  min="0"
                  required
                />
              </div>

              <div class="form-group">
                <label class="form-label">
                  <i class="fas fa-warehouse"></i> Số lượng
                  <span class="required">*</span>
                </label>
                <input
                  v-model.number="product.quantity"
                  type="number"
                  class="form-control"
                  placeholder="Số lượng"
                  disabled
                />
              </div>

              <div class="form-group">
                <label class="form-label">
                  <i class="fas fa-list"></i> Danh mục
                  <span class="required">*</span>
                </label>
                <select
                  v-model.number="product.categoryId"
                  class="form-control"
                  required
                >
                  <option value="">Chọn danh mục</option>
                  <option v-for="cat in categories" :key="cat.loaiSanPhamId" :value="cat.loaiSanPhamId">
                    {{ cat.tenLoai }}
                  </option>
                </select>
              </div>

              <div class="form-group full-width">
                <label class="form-label">
                  <i class="fas fa-align-left"></i> Mô tả
                </label>
                <textarea
                  v-model="product.description"
                  class="form-control"
                  rows="3"
                  placeholder="Nhập mô tả sản phẩm"
                ></textarea>
              </div>

              <div class="form-group full-width">
                <label class="form-label">
                  <i class="fas fa-image"></i> Ảnh sản phẩm
                </label>
                <div class="image-upload-container">
                  <input
                    type="file"
                    class="form-control file-input"
                    @change="handleImage"
                    accept="image/*"
                    id="imageUpload"
                  />
                  <label for="imageUpload" class="file-upload-label">
                    <i class="fas fa-cloud-upload-alt"></i>
                    Chọn ảnh sản phẩm
                  </label>
                  <div v-if="imagePreview" class="image-preview">
                    <img :src="imagePreview" alt="Preview" />
                    <button
                      type="button"
                      @click="removeImage"
                      class="remove-image-btn"
                    >
                      <i class="fas fa-times"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <div class="form-actions">
              <button type="submit" class="btn btn-success" :disabled="loading">
                <i class="fas fa-save"></i>
                {{ loading ? "Đang lưu..." : "Cập nhật" }}
              </button>
              <button
                style="margin-left: 5px"
                type="button"
                @click="resetForm"
                class="btn btn-secondary"
              >
                <i class="fas fa-undo"></i>
                Hủy
              </button>
            </div>
          </form>
        </div>
      </div>

      <!-- Products List Section -->
      <div class="card products-list-section">
        <div class="card-header">
          <h2 class="section-title">
            <i class="fas fa-list"></i>
            Danh sách sản phẩm
            <span class="badge">{{ totalItems }}</span>
          </h2>
          <div class="view-controls">
            <button
              @click="viewMode = 'table'"
              class="btn btn-sm"
              :class="
                viewMode === 'table' ? 'btn-info' : 'btn-outline-secondary'
              "
            >
              <i class="fas fa-table"></i>
              Bảng
            </button>
            <button
              @click="viewMode = 'grid'"
              class="btn btn-sm"
              :class="
                viewMode === 'grid' ? 'btn-info' : 'btn-outline-secondary'
              "
            >
              <i class="fas fa-th"></i>
              Lưới
            </button>
          </div>
        </div>

        <div class="card-body">
          <!-- Loading State -->
          <div v-if="loading" class="loading-state">
            <i class="fas fa-spinner fa-spin"></i>
            Đang tải sản phẩm...
          </div>

          <!-- Empty State -->
          <div v-else-if="!products.length" class="empty-state">
            <i class="fas fa-box-open"></i>
            <p>{{ hasActiveFilters ? 'Không tìm thấy sản phẩm phù hợp với bộ lọc' : 'Không có sản phẩm nào' }}</p>
            <button v-if="hasActiveFilters" @click="resetAndRefresh" class="btn btn-primary">
              <i class="fas fa-times"></i>
              Xóa bộ lọc
            </button>
          </div>

          <!-- Table View -->
          <div v-else-if="viewMode === 'table'" class="table-responsive">
            <table class="products-table">
              <thead>
                <tr>
                  <th>
                    <i class="fas fa-tag"></i>
                    Tên sản phẩm
                  </th>
                  <th>
                    <i class="fas fa-image"></i>
                    Ảnh
                  </th>
                  <th>
                    <i class="fas fa-money-bill-wave"></i>
                    Giá
                  </th>
                  <th>
                    <i class="fas fa-warehouse"></i>
                    Số lượng
                  </th>
                  <th>
                    <i class="fas fa-list"></i>
                    Danh mục
                  </th>
                  <th>
                    <i class="fas fa-align-left"></i>
                    Mô tả
                  </th>
                  <th>
                    <i class="fas fa-tools"></i>
                    Hành động
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="sp in products"
                  :key="sp.sanPhamId"
                  class="product-row"
                >
                  <td class="product-name">
                    <div class="name-container">
                      <strong>{{ sp.tenSP }}</strong>
                    </div>
                  </td>
                  <td class="product-image">
                    <div class="image-container">
                      <img
                        v-if="sp.hinhAnh"
                        :src="sp.hinhAnh"
                        :alt="sp.tenSP"
                        @error="handleImageError"
                      />
                      <div v-else class="no-image">
                        <i class="fas fa-image"></i>
                        <span>Không có ảnh</span>
                      </div>
                    </div>
                  </td>
                  <td class="product-price">
                    <span class="price-value">
                      {{ formatPrice(sp.gia) }}
                    </span>
                  </td>
                  <td class="product-quantity">
                    <span
                      class="quantity-badge"
                      :class="getQuantityClass(sp.soLuong)"
                    >
                      {{ sp.soLuong }}
                    </span>
                  </td>
                  <td class="product-category">
                    <span class="category-badge">
                      {{ getCategoryName(sp.loaiSanPhamId) }}
                    </span>
                  </td>
                  <td class="product-description">
                    <div class="description-text">
                      {{ sp.moTa || "(Không có mô tả)" }}
                    </div>
                  </td>
                  <td class="product-actions">
                    <div class="action-buttons">
                      <button
                        @click="editProduct(sp)"
                        class="btn btn-sm btn-warning"
                        title="Chỉnh sửa"
                      >
                        <i class="fas fa-edit"></i>
                      </button>
                      <button
                        @click="deleteProduct(sp.sanPhamId)"
                        class="btn btn-sm btn-danger"
                        title="Xóa"
                      >
                        <i class="fas fa-trash"></i>
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Grid View -->
          <div v-else class="products-grid">
            <div
              v-for="sp in products"
              :key="sp.sanPhamId"
              class="product-card"
            >
              <div class="product-card-image">
                <img
                  :src="sp.hinhAnh || ''"
                  :alt="sp.tenSP"
                  @error="handleImageError"
                />
                <div class="product-overlay">
                  <button
                    @click="editProduct(sp)"  
                    class="btn btn-sm btn-warning"
                  >
                    <i class="fas fa-edit"></i>
                  </button>
                  <button
                    @click="deleteProduct(sp.sanPhamId)"
                    class="btn btn-sm btn-danger"
                  >
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </div>
              <div class="product-card-content">
                <h5 class="product-card-title">{{ sp.tenSP }}</h5>
                <p class="product-card-price">{{ formatPrice(sp.gia) }}</p>
                <div class="product-card-info">
                  <span class="quantity-info">
                    <i class="fas fa-warehouse"></i>
                    {{ sp.soLuong }}
                  </span>
                  <span class="category-info">
                    <i class="fas fa-tag"></i>
                    {{ getCategoryName(sp.loaiSanPhamId) }}
                  </span>
                </div>
                <p class="product-card-description">
                  {{ sp.moTa || "Không có mô tả" }}
                </p>
              </div>
            </div>
          </div>

          <!-- Pagination -->
          <div v-if="totalPages > 1" class="pagination-container">
            <button
              class="btn btn-outline-primary"
              :disabled="currentPage === 1"
              @click="changePage(currentPage - 1)"
            >
              <i class="fas fa-angle-left"></i> Trước
            </button>

            <span class="page-info">Trang {{ currentPage }} / {{ totalPages }}</span>

            <div class="page-numbers">
              <button
                v-for="page in getVisiblePages()"
                :key="page"
                class="btn btn-sm page-btn"
                :class="page === currentPage ? 'btn-primary' : 'btn-outline-secondary'"
                @click="changePage(page)"
              >
                {{ page }}
              </button>
            </div>

            <button
              class="btn btn-outline-primary"
              :disabled="currentPage === totalPages"
              @click="changePage(currentPage + 1)"
            >
              Sau <i class="fas fa-angle-right"></i>
            </button>
          </div>
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

    <!-- Confirmation Dialog -->
    <div v-if="showConfirmDelete" class="modal" tabindex="-1" role="dialog">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Xác nhận xóa</h5>
            <button
              type="button"
              class="btn-close"
              @click="cancelDelete"
            ></button>
          </div>
          <div class="modal-body">
            <p>Bạn có muốn xóa sản phẩm này không?</p>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              @click="cancelDelete"
            >
              Hủy
            </button>
            <button
              type="button"
              class="btn btn-danger"
              @click="confirmDelete"
            >
              Xóa
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import apiClient from "../utils/axiosClient";

// Reactive data
const products = ref([]);
const categories = ref([]);
const loading = ref(false);
const showEditForm = ref(false);
const viewMode = ref("table");
const toasts = ref([]);
const showConfirmDelete = ref(false);
const productToDelete = ref(null);

// Form data
const product = ref({
  productId: 0,
  productName: "",
  price: 0,
  quantity: 0,
  description: "",
  categoryId: null,
});

// Filters
const searchName = ref("");
const priceMin = ref(null);
const priceMax = ref(null);
const selectedCategory = ref("");

// Debounce timers
let searchTimer = null;
let filterTimer = null;

// Image handling
const imageFile = ref(null);
const imagePreview = ref(null);

// Pagination
const currentPage = ref(1);
const pageSize = ref(5);
const totalItems = ref(0);
const totalPages = computed(() => Math.ceil(totalItems.value / pageSize.value));

// Computed properties
const hasActiveFilters = computed(() => {
  return searchName.value || priceMin.value || priceMax.value || selectedCategory.value;
});

// Methods
const showToast = (message, type = "info") => {
  const toast = {
    id: Date.now(),
    message,
    type,
  };
  toasts.value.push(toast);
  setTimeout(() => {
    const index = toasts.value.findIndex((t) => t.id === toast.id);
    if (index > -1) toasts.value.splice(index, 1);
  }, 3000);
};

const getToastIcon = (type) => {
  switch (type) {
    case "success":
      return "fa-check-circle";
    case "error":
      return "fa-exclamation-circle";
    case "warning":
      return "fa-exclamation-triangle";
    default:
      return "fa-info-circle";
  }
};

// Debounce search function
const debounceSearch = () => {
  if (searchTimer) clearTimeout(searchTimer);
  searchTimer = setTimeout(() => {
    currentPage.value = 1; // Reset to first page when searching
    fetchProducts(1);
  }, 500); // Wait 500ms after user stops typing
};

// Debounce filter function
const debounceFilter = () => {
  if (filterTimer) clearTimeout(filterTimer);
  filterTimer = setTimeout(() => {
    currentPage.value = 1; // Reset to first page when filtering
    fetchProducts(1);
  }, 300); // Wait 300ms after user stops typing
};

// Clear individual filters
const clearSearchFilter = () => {
  searchName.value = "";
  currentPage.value = 1; // Reset về trang 1
  fetchProducts(1);
};

const clearPriceFilter = () => {
  priceMin.value = null;
  priceMax.value = null;
  currentPage.value = 1; // Reset về trang 1
  fetchProducts(1);
};

const clearCategoryFilter = () => {
  selectedCategory.value = "";
  currentPage.value = 1; // Reset về trang 1
  fetchProducts(1);
};

// Format price range for display
const formatPriceRange = () => {
  if (priceMin.value && priceMax.value) {
    return `${formatPrice(priceMin.value)} - ${formatPrice(priceMax.value)}`;
  } else if (priceMin.value) {
    return `Từ ${formatPrice(priceMin.value)}`;
  } else if (priceMax.value) {
    return `Đến ${formatPrice(priceMax.value)}`;
  }
  return "";
};

// Get visible page numbers for pagination
const getVisiblePages = () => {
  const pages = [];
  const start = Math.max(1, currentPage.value - 2);
  const end = Math.min(totalPages.value, start + 4);
  
  for (let i = start; i <= end; i++) {
    pages.push(i);
  }
  return pages;
};

const toggleEditForm = () => {
  showEditForm.value = !showEditForm.value;
  if (!showEditForm.value) resetForm();
};

const handleImage = (e) => {
  const file = e.target.files[0];
  if (file) {
    imageFile.value = file;
    const reader = new FileReader();
    reader.onload = (e) => {
      imagePreview.value = e.target.result;
    };
    reader.readAsDataURL(file);
  }
};

const removeImage = () => {
  imageFile.value = null;
  imagePreview.value = null;
  document.getElementById("imageUpload").value = "";
};

const handleImageError = (e) => {
  e.target.style.display = "none";
  e.target.nextElementSibling?.classList.add("show");
};

const formatPrice = (price) => {
  return price ? price.toLocaleString("vi-VN") + " VND" : "0 VND";
};

const getQuantityClass = (quantity) => {
  if (quantity === 0) return "quantity-zero";
  if (quantity < 10) return "quantity-low";
  return "quantity-normal";
};

const getCategoryName = (categoryId) => {
  const category = categories.value.find((cat) => cat.loaiSanPhamId === categoryId);
  return category ? category.tenLoai : "Không xác định";
};

// Fetch products with improved error handling
const fetchProducts = async (page = 1) => {
  try {
    loading.value = true;
    
    // Build parameters object - đảm bảo đúng với backend API
    const params = {
      page,
      pageSize: pageSize.value
    };
    
    // Add filters only if they have values
    if (searchName.value && searchName.value.trim()) {
      params.keyword = searchName.value.trim();
    }
    
    if (priceMin.value !== null && priceMin.value !== "") {
      params.minPrice = priceMin.value;
    }
    
    if (priceMax.value !== null && priceMax.value !== "") {
      params.maxPrice = priceMax.value;
    }
    
    if (selectedCategory.value && selectedCategory.value !== "") {
      params.categoryId = parseInt(selectedCategory.value); // Đảm bảo là integer
    }

    console.log("Fetching with params:", params); // Debug log

    const res = await apiClient.get("Product/paged", { params });

    // Cập nhật theo cấu trúc response từ backend (PascalCase)
    products.value = res.Items || res.items || [];
    totalItems.value = res.TotalItems || res.totalItems || 0;
    currentPage.value = res.Page || res.page || 1;
    
    // Show toast if filters are applied and no results found
    if (hasActiveFilters.value && products.value.length === 0) {
      showToast("Không tìm thấy sản phẩm phù hợp với bộ lọc", "info");
    }
    
  } catch (err) {
    console.error("Lỗi tải sản phẩm:", err);
    products.value = [];
    totalItems.value = 0;
    showToast("Lỗi khi tải danh sách sản phẩm: " + (err.message || "Không xác định"), "error");
  } finally {
    loading.value = false;
  }
};

const fetchCategories = async () => {
  try {
    const res = await apiClient.get("category");
    categories.value = res;
  } catch (err) {
    console.error("Fetch categories error:", err);
    showToast("Lỗi khi tải danh sách danh mục: " + (err.message || "Không xác định"), "error");
  }
};

// Filter functions
const clearSearch = () => {
  searchName.value = "";
  currentPage.value = 1;
  fetchProducts(1);
};

const filterByCategory = () => {
  currentPage.value = 1;
  fetchProducts(1);
};

const resetAndRefresh = async () => {
  searchName.value = "";
  priceMin.value = null;
  priceMax.value = null;
  selectedCategory.value = "";
  currentPage.value = 1;
  await fetchProducts(1);
  showToast("Đã xóa tất cả bộ lọc", "success");
};

// Pagination
const changePage = (page) => {
  if (page >= 1 && page <= totalPages.value && page !== currentPage.value) {
    fetchProducts(page);
  }
};



// Thêm biến để lưu giá gốc
const originalPrice = ref(null);

// Sửa hàm editProduct để lưu giá gốc
const editProduct = (sp) => {
  product.value = {
    productId: sp.sanPhamId,
    productName: sp.tenSP,
    price: sp.gia,
    quantity: sp.soLuong,
    description: sp.moTa,
    categoryId: sp.loaiSanPhamId,
  };
  originalPrice.value = sp.gia; // Lưu giá gốc
  imagePreview.value = sp.hinhAnh || null;
  imageFile.value = null;
  showEditForm.value = true;
};

// Sửa hàm saveProduct
const saveProduct = async () => {
  try {
    loading.value = true;
    const formData = new FormData();
    
    // Chỉ thêm các field có dữ liệu mới (không rỗng, không null, không undefined)
    if (product.value.productName && product.value.productName.trim()) {
      formData.append("productName", product.value.productName.trim());
    }
    
    // Luôn gửi giá nếu có giá trị (cho phép giá = 0)
    if (product.value.price != null) {
      formData.append("sellingPrice", product.value.price); // Sửa key thành "sellingPrice"
    }
    
    // Đối với mô tả, chỉ cập nhật nếu có nội dung (cho phép cập nhật thành rỗng)
    if (product.value.description !== null && product.value.description !== undefined) {
      formData.append("description", product.value.description);
    }
    
    // Đối với danh mục, chỉ cập nhật nếu đã chọn danh mục mới
    if (product.value.categoryId) {
      formData.append("categoryId", product.value.categoryId);
    }
    
    // Chỉ thêm ảnh mới nếu có chọn file mới
    if (imageFile.value) {
      formData.append("image", imageFile.value);
    }

    // Debug FormData
    for (let [key, value] of formData.entries()) {
      console.log(key, value);
    }

    // Kiểm tra xem có dữ liệu nào để cập nhật không
    let hasDataToUpdate = false;
    for (let pair of formData.entries()) {
      hasDataToUpdate = true;
      break;
    }
    
    if (!hasDataToUpdate) {
      showToast("Không có dữ liệu mới để cập nhật!", "warning");
      return;
    }

    const url = `Product/${product.value.productId}`;
    
    await apiClient.put(url, formData, { 
      headers: { "Content-Type": "multipart/form-data" } 
    });
    
    showToast("Cập nhật sản phẩm thành công!", "success");

    resetForm();
    await fetchProducts(currentPage.value); // Keep current page
    showEditForm.value = false;
    
  } catch (err) {
    console.error("Lỗi lưu sản phẩm:", err);
    showToast("Lỗi khi lưu sản phẩm: " + (err.response?.data?.message || err.message || "Không xác định"), "error");
  } finally {
    loading.value = false;
  }
};


const deleteProduct = (id) => {
  showConfirmDelete.value = true;
  productToDelete.value = id;
};

const confirmDelete = async () => {
  try {
    loading.value = true;
    await apiClient.delete(`Product/${productToDelete.value}`);
    showToast("Xóa sản phẩm thành công!", "success");
    
    // If current page becomes empty after deletion, go to previous page
    if (products.value.length === 1 && currentPage.value > 1) {
      await fetchProducts(currentPage.value - 1);
    } else {
      await fetchProducts(currentPage.value);
    }
  } catch (err) {
    console.error("Lỗi xóa:", err);
    showToast("Lỗi khi xóa sản phẩm: " + (err.message || "Không xác định"), "error");
  } finally {
    loading.value = false;
    showConfirmDelete.value = false;
    productToDelete.value = null;
  }
};

const cancelDelete = () => {
  showConfirmDelete.value = false;
  productToDelete.value = null;
};

const resetForm = () => {
  product.value = {
    productId: 0,
    productName: "",
    price: 0,
    quantity: 0,
    description: "",
    categoryId: null,
  };
  imageFile.value = null;
  imagePreview.value = null;
  if (document.getElementById("imageUpload")) {
    document.getElementById("imageUpload").value = "";
  }
  showEditForm.value = false;
};

// Lifecycle
onMounted(async () => {
  await Promise.all([fetchProducts(), fetchCategories()]);
});
</script>

<style scoped>
/* Base styles */
.product-management {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
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
  color: #e67e22;
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

.badge {
  background: rgba(255, 255, 255, 0.2);
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 0.9rem;
  margin-left: 10px;
}

.card-body {
  padding: 25px;
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

.form-label {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 8px;
  font-size: 0.95rem;
  display: flex;
  align-items: center;
  gap: 8px;
}

.form-label i {
  color: #3498db;
  font-size: 1rem;
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

.form-control:disabled {
  background: #e1e8ed;
  cursor: not-allowed;
}

/* Search Container */
.search-container {
  position: relative;
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
  color: rgba(0, 0, 0, 0.6);
  cursor: pointer;
  padding: 5px;
  border-radius: 50%;
  transition: all 0.3s ease;
  display: none;
}

.clear-search.show {
  display: inline-block;
}

.clear-search:hover {
  background: rgba(231, 76, 60, 0.1);
  color: #e74c3c;
}

.price-filter-group {
  display: flex;
  align-items: center;
  gap: 10px;
}

.price-input {
  width: 100%;
}

.price-separator {
  font-weight: bold;
  font-size: 1.2rem;
  color: #7f8c8d;
}

/* Filter Status */
.filter-status {
  display: flex;
  align-items: center;
  gap: 10px;
}

.filter-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: 500;
  background: #ecf0f1;
  color: #7f8c8d;
}

.filter-badge.active {
  background: linear-gradient(135deg, #3498db, #2980b9);
  color: white;
}

/* Active Filters */
.active-filters {
  background: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 8px;
  padding: 15px;
  margin-top: 20px;
}

.active-filters h4 {
  margin: 0 0 10px 0;
  font-size: 1rem;
  color: #495057;
}

.filter-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.filter-tag {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background: #e9ecef;
  color: #495057;
  padding: 6px 12px;
  border-radius: 16px;
  font-size: 0.85rem;
  border: 1px solid #ced4da;
}

.filter-tag i {
  color: #6c757d;
}

.remove-filter {
  background: none;
  border: none;
  color: #dc3545;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
  margin-left: 4px;
  padding: 0 4px;
  border-radius: 50%;
  transition: all 0.2s ease;
}

.remove-filter:hover {
  background: #dc3545;
  color: white;
}

/* Toast Notification */
.toast-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.toast {
  padding: 12px 20px;
  border-radius: 8px;
  font-size: 0.95rem;
  color: white;
  display: flex;
  align-items: center;
  gap: 10px;
  animation: fadeInOut 3s ease forwards;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.toast.info {
  background: #3498db;
}
.toast.success {
  background: #2ecc71;
}
.toast.warning {
  background: #f39c12;
}
.toast.error {
  background: #e74c3c;
}

@keyframes fadeInOut {
  0% {
    opacity: 0;
    transform: translateY(20px);
  }
  10% {
    opacity: 1;
    transform: translateY(0);
  }
  90% {
    opacity: 1;
    transform: translateY(0);
  }
  100% {
    opacity: 0;
    transform: translateY(20px);
  }
}

/* Table styles */
.table-responsive {
  overflow-x: auto;
}

.products-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  border-radius: 10px;
  overflow: hidden;
  min-width: 800px;
}

.products-table th,
.products-table td {
  padding: 14px 16px;
  text-align: left;
  border-bottom: 1px solid #e1e8ed;
  font-size: 0.95rem;
}

.products-table th {
  background: #f7f9fb;
  font-weight: 600;
  color: #34495e;
  white-space: nowrap;
}

.products-table tr:hover {
  background: #f0f8ff;
}

.image-container {
  display: flex;
  justify-content: center;
}

.image-container img {
  width: 60px;
  height: 60px;
  object-fit: cover;
  border-radius: 8px;
}

.no-image {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 60px;
  height: 60px;
  background: #f8f9fa;
  border: 1px dashed #dee2e6;
  border-radius: 8px;
  font-size: 0.75rem;
  color: #6c757d;
}

.no-image i {
  font-size: 1.2rem;
  margin-bottom: 2px;
}

.quantity-badge {
  display: inline-block;
  padding: 5px 10px;
  border-radius: 12px;
  font-weight: 600;
  font-size: 0.85rem;
  color: white;
}

.quantity-zero {
  background: #e74c3c;
}

.quantity-low {
  background: #f39c12;
}

.quantity-normal {
  background: #2ecc71;
}

.category-badge {
  background: #bdc3c7;
  padding: 4px 10px;
  border-radius: 12px;
  font-size: 0.85rem;
  color: #2c3e50;
}

.price-value {
  font-weight: 600;
  color: #e67e22;
}

.description-text {
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Grid view */
.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 20px;
}

.product-card {
  background: white;
  border: 1px solid #e1e8ed;
  border-radius: 12px;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
}

.product-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.product-card-image {
  position: relative;
  height: 200px;
  overflow: hidden;
}

.product-card-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.product-overlay {
  position: absolute;
  bottom: 10px;
  right: 10px;
  display: flex;
  gap: 8px;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.product-card:hover .product-overlay {
  opacity: 1;
}

.product-card-content {
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 8px;
  flex-grow: 1;
}

.product-card-title {
  font-weight: 600;
  font-size: 1.1rem;
  color: #2c3e50;
  margin: 0;
  line-height: 1.3;
}

.product-card-price {
  color: #e67e22;
  font-weight: bold;
  font-size: 1.2rem;
  margin: 0;
}

.product-card-info {
  display: flex;
  justify-content: space-between;
  font-size: 0.85rem;
  color: #7f8c8d;
  margin: 8px 0;
}

.quantity-info,
.category-info {
  display: flex;
  align-items: center;
  gap: 4px;
}

.product-card-description {
  font-size: 0.9rem;
  color: #34495e;
  line-height: 1.4;
  margin: 0;
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
}

/* Loading and Empty state */
.loading-state,
.empty-state {
  text-align: center;
  padding: 50px 20px;
  color: #7f8c8d;
  font-size: 1.1rem;
}

.loading-state i {
  font-size: 2rem;
  margin-bottom: 10px;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 15px;
}

.empty-state i {
  font-size: 3rem;
  color: #bdc3c7;
}

.view-controls {
  display: flex;
  gap: 5px;
}

.view-controls .btn {
  border-radius: 6px;
}

/* Pagination */
.pagination-container {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 12px;
  margin-top: 30px;
  flex-wrap: wrap;
}

.page-info {
  font-weight: 500;
  color: #495057;
  white-space: nowrap;
}

.page-numbers {
  display: flex;
  gap: 4px;
  flex-wrap: wrap;
}

.page-btn {
  min-width: 40px;
  height: 36px;
  border-radius: 6px;
  font-size: 0.9rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Action Buttons */
.action-buttons {
  display: flex;
  gap: 6px;
  justify-content: flex-start;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  transition: all 0.3s ease;
  text-decoration: none;
  min-height: 36px;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-sm {
  padding: 6px 12px;
  font-size: 0.8rem;
  min-height: 32px;
}

.btn-primary {
  background: linear-gradient(135deg, #3498db, #2980b9);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(52, 152, 219, 0.3);
}

.btn-success {
  background: linear-gradient(135deg, #27ae60, #229954);
  color: white;
}

.btn-success:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(39, 174, 96, 0.3);
}

.btn-secondary {
  background: linear-gradient(135deg, #95a5a6, #7f8c8d);
  color: white;
}

.btn-info {
  background: linear-gradient(135deg, #3498db, #2980b9);
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

.btn-outline-secondary {
  background: transparent;
  color: #6c757d;
  border: 2px solid #6c757d;
}

.btn-outline-secondary:hover:not(:disabled) {
  background: #6c757d;
  color: white;
}

/* Modal */
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-dialog {
  background: white;
  border-radius: 12px;
  width: 400px;
  max-width: 90%;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

.modal-content {
  padding: 0;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 25px;
  border-bottom: 1px solid #e1e8ed;
}

.modal-title {
  font-size: 1.3rem;
  font-weight: 600;
  color: #2c3e50;
  margin: 0;
}

.btn-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #7f8c8d;
  cursor: pointer;
  padding: 4px;
  border-radius: 50%;
  transition: all 0.3s ease;
}

.btn-close:hover {
  background: rgba(231, 76, 60, 0.1);
  color: #e74c3c;
}

.modal-body {
  padding: 25px;
  color: #495057;
  line-height: 1.5;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 20px 25px;
  border-top: 1px solid #e1e8ed;
}

/* Form Actions */
.form-actions {
  display: flex;
  gap: 10px;
  margin-top: 20px;
  flex-wrap: wrap;
}

/* Image Upload */
.image-upload-container {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.file-input {
  display: none;
}

.file-upload-label {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 12px 20px;
  background: linear-gradient(135deg, #3498db, #2980b9);
  color: white;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 0.95rem;
  font-weight: 500;
  max-width: 200px;
}

.file-upload-label:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(52, 152, 219, 0.3);
}

.image-preview {
  position: relative;
  display: inline-block;
  max-width: 200px;
}

.image-preview img {
  width: 100%;
  height: 150px;
  object-fit: cover;
  border-radius: 8px;
  border: 2px solid #e1e8ed;
}

.remove-image-btn {
  position: absolute;
  top: -8px;
  right: -8px;
  background: #e74c3c;
  color: white;
  border: none;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  transition: all 0.3s ease;
}

.remove-image-btn:hover {
  background: #c0392b;
  transform: scale(1.1);
}

/* Responsive Design */
@media (max-width: 768px) {
  .product-management {
    padding: 15px;
  }
  
  .title {
    font-size: 2rem;
  }
  
  .form-grid {
    grid-template-columns: 1fr;
    gap: 15px;
  }
  
  .products-grid {
    grid-template-columns: 1fr;
  }
  
  .pagination-container {
    gap: 8px;
  }
  
  .page-numbers {
    gap: 2px;
  }
  
  .btn {
    padding: 10px 14px;
    font-size: 0.85rem;
  }
  
  .modal-dialog {
    margin: 20px;
    width: auto;
  }
  
  .card-header {
    padding: 15px 20px;
  }
  
  .card-body {
    padding: 20px;
  }
  
  .section-title {
    font-size: 1.2rem;
  }
  
  .filter-tags {
    justify-content: center;
  }
  
  .active-filters {
    text-align: center;
  }
}

@media (max-width: 480px) {
  .view-controls {
    order: -1;
    width: 100%;
    justify-content: center;
    margin-bottom: 10px;
  }
  
  .card-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }
  
  .price-filter-group {
    flex-direction: column;
    gap: 8px;
  }
  
  .price-separator {
    text-align: center;
  }
}
</style>