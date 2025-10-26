<script setup>
import * as XLSX from "xlsx";
import { ref } from "vue";
import dayjs from "dayjs";

const selectedSheet = ref("T10");
const selectedDate = ref(dayjs().subtract(1, "day").format("YYYY-MM-DD"));

const reportText = ref("");
const deXuat = ref("");

let workbook = null;

// === Các trường nhập tay ===
const dongChayCoc = ref(13);

// Thay đổi: dùng object có 2 số
const thungTG = ref({ so1: 3, so2: 4 });
const thungRot = ref({ so1: 2, so2: 4 });

const phoiDauDuoi = ref(0);
const phoiThieuNhiet = ref(0);
const phoiSuCoDuc = ref(0);
const phoiSuCoCan = ref(0);
const nhanLucNauLuyen = ref(10);
const nhanLucDucDong = ref(8);
// ===== Helpers =====
const copied = ref(false);

const copyReport = async () => {
  try {
    await navigator.clipboard.writeText(reportText.value);
    copied.value = true;
    setTimeout(() => {
      copied.value = false;
    }, 2000); // Sau 2s thì reset nút
  } catch (err) {
    alert("⚠️ Không thể copy: " + err);
  }
};
const formatDateDDMMYYYY = (d) => {
  if (!(d instanceof Date)) return "";
  const dd = String(d.getDate()).padStart(2, "0");
  const mm = String(d.getMonth() + 1).padStart(2, "0");
  const yyyy = d.getFullYear();
  return `${dd}/${mm}/${yyyy}`;
};

const normalize = (s) => {
  if (s === undefined || s === null) return "";
  try {
    return String(s)
      .normalize("NFD")
      .replace(/[\u0300-\u036f]/g, "")
      .replace(/[^\w\s]/g, " ")
      .replace(/\s+/g, " ")
      .trim()
      .toLowerCase();
  } catch {
    return String(s).toLowerCase();
  }
};

const getCellText = (sheet, r, c) => {
  const addr = XLSX.utils.encode_cell({ r, c });
  const cell = sheet[addr];
  if (!cell) return "";
  if (cell.w !== undefined && cell.w !== null) return String(cell.w).trim();
  if (cell.t === "d" && cell.v instanceof Date)
    return formatDateDDMMYYYY(cell.v);
  if (cell.v !== undefined && cell.v !== null) return String(cell.v).trim();
  return "";
};

function parseNumberFromString(str) {
  if (!str) return 0;
  let val = str.trim();
  if (val.includes(",") && !val.includes(".")) {
    val = val.replace(/,/g, "");
    return parseFloat(val);
  }
  if (val.includes(".") && !val.includes(",")) {
    return parseFloat(val);
  }
  if (val.includes(".") && val.includes(",")) {
    val = val.replace(/\./g, "");
    val = val.replace(",", ".");
    return parseFloat(val);
  }
  return parseFloat(val);
}

function findValue(sheet, startRow, range, keywords, opt = {}) {
  const { offsetRow = 0, offsetCol = 0, parse = true, fallback = null } = opt;

  for (let r = startRow; r <= Math.min(startRow + 22, range.e.r); r++) {
    for (let c = range.s.c; c <= range.e.c; c++) {
      const txtNorm = normalize(getCellText(sheet, r, c));
      if (keywords.some((kw) => txtNorm.includes(kw))) {
        let valText = getCellText(sheet, r + offsetRow, c + offsetCol);
        console.log(
          "🔍 Tìm thấy keyword",
          keywords,
          "tại dòng",
          r + 1,
          "cột",
          XLSX.utils.encode_col(c),
          "=> Giá trị tại offset:",
          getCellText(sheet, r + offsetRow, c + offsetCol)
        );
        if (!valText) valText = getCellText(sheet, r, c - 1);
        return parse ? parseNumberFromString(valText) : valText.trim();
      }
    }
  }
  return fallback;
}

// === Hàm lọc tình hình sản xuất ===
function cleanTinhHinhSanXuat(text) {
  if (!text) return "";
  let lines = text
    .split("\n")
    .map((l) => l.trim())
    .filter((l) => l);

  // Bỏ dòng đầu (Tổng số mẻ nấu luyện)
  if (lines[0] && lines[0].includes("Tổng số mẻ nấu luyện")) {
    lines.shift();
  }

  // Tìm vị trí "Tổng khối lượng ferro" để cắt
  let ferroIndex = lines.findIndex((l) => l.includes("Tổng khối lượng ferro"));
  if (ferroIndex >= 0) {
    lines = lines.slice(0, ferroIndex);
  }

  return lines.join("\n");
}

// ===== Upload file =====
const handleFileUpload = (event) => {
  const file = event.target.files[0];
  if (!file) return;
  const reader = new FileReader();
  reader.onload = (e) => {
    const data = new Uint8Array(e.target.result);
    workbook = XLSX.read(data, { type: "array", cellDates: true });
  };
  reader.readAsArrayBuffer(file);
};

// ===== Main =====
const generateReport = () => {
  if (!workbook) {
    alert("⚠️ Vui lòng upload file Excel trước!");
    return;
  }

  const sheetName = selectedSheet.value;
  const sheet = workbook.Sheets[sheetName];
  if (!sheet) {
    alert("⚠️ Không tìm thấy sheet: " + sheetName);
    return;
  }

  const targetDate = new Date(selectedDate.value);
  if (isNaN(targetDate.getTime())) {
    alert("⚠️ Ngày không hợp lệ");
    return;
  }
  const dateText = formatDateDDMMYYYY(targetDate);
  const dateTextNorm = normalize(dateText);

  const rangeRef = sheet["!ref"];
  if (!rangeRef) {
    alert("⚠️ Sheet rỗng hoặc không có dữ liệu");
    return;
  }
  const range = XLSX.utils.decode_range(rangeRef);

  // --- B1: tìm dòng chứa ngày ---
  let foundDateRow = -1;
  for (let r = range.s.r; r <= range.e.r; r++) {
    for (let c = range.s.c; c <= range.e.c; c++) {
      if (normalize(getCellText(sheet, r, c)).includes(dateTextNorm)) {
        foundDateRow = r;
        break;
      }
    }
    if (foundDateRow !== -1) break;
  }

  if (foundDateRow === -1) {
    reportText.value = `⚠️ Không tìm thấy ngày ${dateText}`;
    return;
  }

  // --- B2: Lấy dữ liệu từ Excel ---
  const tongMe = findValue(sheet, foundDateRow, range, ["so me ngay"], {
    offsetCol: 3,
  });
  const mnsi = findValue(sheet, foundDateRow, range, ["s mnsi kg ngay"], {
    offsetCol: 2,
  });
  const fesi = findValue(sheet, foundDateRow, range, ["s fesi kg ngay"], {
    offsetCol: 2,
  });
  const femn = findValue(sheet, foundDateRow, range, ["s femn kg ngay"], {
    offsetCol: 2,
  });
  const sanluong = findValue(sheet, foundDateRow, range, ["tong trong luong"], {
    offsetRow: 1,
  });
  const tongcay = findValue(sheet, foundDateRow, range, ["tong cay ngay"], {
    offsetRow: 1,
  });
  const tbtan = findValue(sheet, foundDateRow, range, ["trung binh"], {
    offsetRow: 1,
  });
  const timenauend = findValue(
    sheet,
    foundDateRow,
    range,
    ["bao cao nau luyen"],
    { offsetRow: 1, offsetCol: 18, parse: false }
  );
  const timerotstart = findValue(
    sheet,
    foundDateRow,
    range,
    ["bao cao nau luyen"],
    { offsetRow: -4, offsetCol: 18, parse: false }
  );
  const timerotend = findValue(
    sheet,
    foundDateRow,
    range,
    ["bao cao nau luyen"],
    { offsetRow: -3, offsetCol: 18, parse: false }
  );
  let tinhHinhSanXuat = findValue(sheet, foundDateRow, range, ["ghi chu"], {
    offsetRow: 2,
    parse: false,
  });

  // 👉 Làm sạch tinhHinhSanXuat
  tinhHinhSanXuat = cleanTinhHinhSanXuat(tinhHinhSanXuat);

  // --- B3: Xuất báo cáo ---
  const tgStr = [thungTG.value.so1, thungTG.value.so2]
    .filter(Boolean)
    .join(", ");
  const rotStr = [thungRot.value.so1, thungRot.value.so2]
    .filter(Boolean)
    .join(", ");

  reportText.value = `Phân Xưởng Luyện - Đúc báo cáo BGĐ kết quả SX đêm ${dateText}:
1. PHÂN XƯỞNG NẤU LUYỆN:
- Nhân lực đi làm: ${nhanLucNauLuyen.value}/10
- Tổng số mẻ: ${tongMe}.
- Chất lượng nước thép: Ổn định.
- Trong ca máy phân tích hoạt động không ổn định khi phân tích những thành phần như (C;Mn;Si).
- SiMn dùng trong ca: ${mnsi} kg.
- FeMn dùng trong ca: ${femn} kg.
- FeSi dùng trong ca: ${fesi} kg.
- Sản lượng trong ca: ${sanluong} tấn.
- Số tấn/1 mẻ: ${tbtan} tấn/1 mẻ.
- Số kg MnSi/1 tấn: ${(mnsi / sanluong).toFixed(2)} kg/1 tấn.
- Số kg FeMn/1 tấn: ${(femn / sanluong).toFixed(2)} kg/1 tấn.
- Số kg FeSi/1 tấn: ${(fesi / sanluong).toFixed(2)} kg/1 tấn.
- Tổng số kg(FeSi,FeMn,MnSi)/1 tấn: ${(
    mnsi / sanluong +
    femn / sanluong +
    fesi / sanluong
  ).toFixed(2)} kg/1 tấn.
- Kết thúc nấu: ${timenauend}.
2. ĐÚC DÒNG:
- Nhân lực đi làm: ${nhanLucDucDong.value}/8.
- ${timerotstart} bắt đầu rót
- Chất lượng phôi thép ổn định.
- Dòng chạy cốc ${dongChayCoc.value}.
- Chạy thùng trung gian số ${tgStr}.
- Chạy thùng rót số ${rotStr}.
- Tổng số phôi: ${tongcay} cây 3m.
     + Phôi đầu đuôi: ${phoiDauDuoi.value} cây
     + Phôi thiếu nhiệt: ${phoiThieuNhiet.value} cây
     + Phôi sự cố đúc: ${phoiSuCoDuc.value} cây
     + Phôi sự cố cán: ${phoiSuCoCan.value} cây.
- Kết thúc sx lúc: ${timerotend}
3. TÌNH HÌNH SẢN XUẤT:
${tinhHinhSanXuat}

4. ĐỀ XUẤT, SỬA CHỮA:
${deXuat.value}`;
};
</script>

<template>
  <div class="container-fluid py-4">
    <div class="row g-3">
      <!-- Cột trái: Thông tin nhập liệu -->
      <div class="col-lg-6">
        <div class="card shadow-sm h-100">
          <div class="card-header bg-primary text-white">
            <h4 class="mb-0">
              <i class="bi bi-file-earmark-excel"></i> Nhập thông tin
            </h4>
          </div>

          <div class="card-body">
            <!-- Upload file -->
            <div class="mb-3">
              <label class="form-label fw-bold">Chọn file Excel:</label>
              <input
                type="file"
                class="form-control"
                @change="handleFileUpload"
                accept=".xlsx,.xls"
              />
            </div>

            <div class="row mb-3">
              <!-- Chọn sheet -->
              <div class="col-md-6">
                <label class="form-label fw-bold">Sheet:</label>
                <input
                  v-model="selectedSheet"
                  class="form-control"
                  placeholder="VD: T9"
                />
              </div>
              <!-- Chọn ngày -->
              <div class="col-md-6">
                <label class="form-label fw-bold">Ngày:</label>
                <input
                  type="date"
                  v-model="selectedDate"
                  class="form-control"
                />
              </div>
            </div>

            <!-- Nhập thủ công -->
            <div class="card-body">
              <div class="row g-3">
                <!-- Hàng 1 -->
                <div class="col-md-4">
                  <label class="form-label">Dòng chạy cốc:</label>
                  <input
                    type="number"
                    v-model="dongChayCoc"
                    class="form-control"
                  />
                </div>

                <div class="col-md-4">
                  <label class="form-label">Thùng TG (2 số):</label>
                  <div class="input-group">
                    <input
                      type="number"
                      v-model="thungTG.so1"
                      class="form-control"
                      placeholder="S1"
                    />
                    <input
                      type="number"
                      v-model="thungTG.so2"
                      class="form-control"
                      placeholder="S2"
                    />
                  </div>
                </div>

                <div class="col-md-4">
                  <label class="form-label">Thùng rót (2 số):</label>
                  <div class="input-group">
                    <input
                      type="number"
                      v-model="thungRot.so1"
                      class="form-control"
                      placeholder="S1"
                    />
                    <input
                      type="number"
                      v-model="thungRot.so2"
                      class="form-control"
                      placeholder="S2"
                    />
                  </div>
                </div>

                <!-- Hàng 2 -->
                <div class="col-md-4">
                  <label class="form-label">Phôi đầu đuôi:</label>
                  <input
                    type="number"
                    v-model="phoiDauDuoi"
                    class="form-control"
                  />
                </div>

                <div class="col-md-4">
                  <label class="form-label">Phôi thiếu nhiệt:</label>
                  <input
                    type="number"
                    v-model="phoiThieuNhiet"
                    class="form-control"
                  />
                </div>

                <div class="col-md-4">
                  <label class="form-label">Phôi sự cố đúc:</label>
                  <input
                    type="number"
                    v-model="phoiSuCoDuc"
                    class="form-control"
                  />
                </div>

                <!-- Hàng 3 -->
                <div class="col-md-4">
                  <label class="form-label">Phôi sự cố cán:</label>
                  <input
                    type="number"
                    v-model="phoiSuCoCan"
                    class="form-control"
                  />
                </div>

                <div class="col-md-4">
                  <label class="form-label">Nhân lực Nấu luyện:</label>
                  <input
                    type="number"
                    v-model="nhanLucNauLuyen"
                    class="form-control"
                    min="0"
                    max="10"
                  />
                </div>

                <div class="col-md-4">
                  <label class="form-label">Nhân lực Đúc dòng:</label>
                  <input
                    type="number"
                    v-model="nhanLucDucDong"
                    class="form-control"
                    min="0"
                    max="8"
                  />
                </div>

                <!-- Hàng cuối -->
                <div class="col-12">
                  <label class="form-label">Đề xuất, Sửa chữa:</label>
                  <textarea
                    v-model="deXuat"
                    rows="3"
                    class="form-control"
                    placeholder="Nhập đề xuất..."
                  ></textarea>
                </div>
              </div>
            </div>

            <div class="d-grid">
              <button @click="generateReport" class="btn btn-success btn-lg">
                <i class="bi bi-file-text"></i> Tạo báo cáo
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Cột phải: Kết quả -->
      <div class="col-lg-6">
        <div class="card shadow-sm h-100">
          <div
            class="card-header bg-success text-white d-flex justify-content-between align-items-center"
          >
            <h4 class="mb-0">
              <i class="bi bi-clipboard-check"></i> Kết quả báo cáo
            </h4>
            <button
              v-if="reportText"
              @click="copyReport"
              class="btn btn-light btn-sm"
              :class="{ 'btn-success': copied }"
            >
              <i :class="copied ? 'bi bi-check-lg' : 'bi bi-clipboard'"></i>
              {{ copied ? "Đã copy!" : "Copy" }}
            </button>
          </div>
          <div class="card-body">
            <div v-if="!reportText" class="text-center text-muted py-5">
              <i class="bi bi-file-text" style="font-size: 3rem"></i>
              <p class="mt-3">
                Chưa có báo cáo. Vui lòng nhập thông tin và nhấn "Tạo báo cáo"
              </p>
            </div>
            <pre v-else class="report-output">{{ reportText }}</pre>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.report-output {
  white-space: pre-wrap;
  background-color: #f8f9fa;
  padding: 1rem;
  border-radius: 0.375rem;
  border: 1px solid #dee2e6;
  font-family: "Courier New", monospace;
  font-size: 0.9rem;
  line-height: 1.6;
  margin: 0;
}

.card {
  border: none;
  border-radius: 0.5rem;
}

.card-header {
  border-radius: 0.5rem 0.5rem 0 0 !important;
}

.form-label {
  font-size: 0.9rem;
  color: #495057;
}

.btn-lg {
  padding: 0.75rem 1.5rem;
  font-size: 1.1rem;
}

/* Bootstrap Icons CDN được thêm trong head nếu cần */
</style>
